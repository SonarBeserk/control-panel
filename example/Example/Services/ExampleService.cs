// Copyright 2025 SonarBeserk
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Grpc.Core;

namespace Example.Services;

using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Data.Sqlite;

public class ExampleService : Example.ExampleBase
{
    private readonly ILogger<ExampleService> _logger;
    private readonly SqliteConnection _conn;

    public ExampleService(ILogger<ExampleService> logger, SqliteConnection conn)
    {
        _logger = logger;
        _conn = conn;
    }

    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
        var command = _conn.CreateCommand();
        command.CommandText = "SELECT naming.nickname FROM naming WHERE name = @name LIMIT 1";
        command.Parameters.AddWithValue("@name", request.Name);

        using var reader = command.ExecuteReader();
        var nickname = request.Name;
        if (reader.Read())
        {
            nickname = reader.GetString(0);
            _logger.LogInformation("Found {Nickname}", nickname);
        }

        // Add stat for greeting
        TrackGreeting(request.Name, null);

        return Task.FromResult(new HelloResponse
        {
            Message = "Hello " + nickname
        });
    }

    public override Task<GetStatsResponse> GetStats(GetStatsRequest request, ServerCallContext context)
    {
        var command = _conn.CreateCommand();
        command.CommandText = "SELECT name, sent_at FROM stats";

        using var reader = command.ExecuteReader();

        var stats = new GetStatsResponse();

        var name = reader.GetOrdinal("name");
        var sentAt = reader.GetOrdinal("sent_at");

        while (reader.Read())
        {
            var time = reader.GetString(sentAt);
            var date = DateTime.Parse(time, CultureInfo.CurrentCulture);

            var stat = new GreetingStat
            {
                Name = reader.GetString(name),
                Sent = Timestamp.FromDateTime(date.ToUniversalTime())
            };

            stats.Greetings.Add(stat);
        }

        return Task.FromResult(stats);
    }

    public override Task<Empty> UpdateNickname(UpdateNicknameRequest request, ServerCallContext context)
    {
        var command = _conn.CreateCommand();
        command.CommandText = "INSERT OR REPLACE INTO naming VALUES (@id, @name, @nickname);";
        command.Parameters.AddWithValue("@id", Guid.NewGuid());
        command.Parameters.AddWithValue("@name", request.Name);
        command.Parameters.AddWithValue("@nickname", request.Nickname);

        try
        {
            var result = command.ExecuteNonQuery();
            if (result != 0)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Failed to update nickname"));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update nickname");
        }

        return Task.FromResult(new Empty());
    }

    /// <summary>
    /// Tracks a greeting and stores it in the database
    /// </summary>
    /// <param name="name">The name that was greeted</param>
    /// <param name="start">The time the greeting was sent, defaults to <see cref="DateTime.Now"/> if not set</param>
    /// <exception cref="RpcException">An error occured when saving the stat</exception>
    public void TrackGreeting(string name, DateTime? start)
    {
        // Set start time if it is not provided
        start ??= DateTime.Now;

        var command =  _conn.CreateCommand();
        command.CommandText = "INSERT INTO Stats (stat_id, name, sent_at) VALUES (@Id, @Name, @SentAt)";
        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@SentAt", start);

        try
        {
            var result = command.ExecuteNonQuery();
            // Should be one row added
            if (result != 1)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Failed to insert stat"));
            }
        }
        catch (SqliteException e)
        {
            _logger.LogError(e, "Failed to track greeting");
        }
    }
}
