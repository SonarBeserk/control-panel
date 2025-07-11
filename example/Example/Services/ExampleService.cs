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

        return Task.FromResult(new HelloResponse
        {
            Message = "Hello " + nickname
        });
    }

    public override Task<GetStatsResponse> GetStats(GetStatsRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetStatsResponse() { });
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
            if (result != 1)
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
}
