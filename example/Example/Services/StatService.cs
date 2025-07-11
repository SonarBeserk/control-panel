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

namespace Example.Services;

using Grpc.Core;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

public class StatService
{
    private readonly ILogger<StatService> _logger;
    private readonly SqliteConnection _conn;

    public StatService(ILogger<StatService> logger, SqliteConnection conn)
    {
        _logger = logger;
        _conn = conn;
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
            if (result != 0)
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
