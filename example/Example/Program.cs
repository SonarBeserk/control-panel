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

using Base.Services;
using Example.Services;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Handle dependency injection
builder.Services.AddSingleton<SqliteConnection>(_ =>
{
    var sqlSetting = builder.Configuration.GetConnectionString("sqlite");
    if (string.IsNullOrWhiteSpace(sqlSetting))
    {
        throw new InvalidOperationException("Database connection string is missing or invalid");
    }

    var conn = new SqliteConnection(sqlSetting);
    conn.Open(); // Open long-running connection to save memory and io calls
    return conn;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ExampleService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Migrate the database
SqlService.UpdateSchema(builder.Configuration.GetConnectionString("sqlite") ?? string.Empty);

app.Run();
