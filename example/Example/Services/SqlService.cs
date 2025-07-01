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

using Microsoft.Data.Sqlite;

public static class SqlService
{
    private const string SqlMigrationsPath = "Sql/";

    /// <summary>
    /// Runs migrations to update the database schema
    /// </summary>
    /// <param name="connectionString">The connection string for the datqbase</param>
    /// <exception cref="InvalidOperationException">Error thrown if the database connection string is invalid</exception>
    public static void UpdateSchema(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is missing or invalid");
        }
        var conn = new SqliteConnection(connectionString);
        conn.Open(); // Open long-running connection to save memory and io calls

        var userVersion = GetUserVersion(conn);
        Console.WriteLine($"Database Version: {userVersion}");

        var migrations = FindMigrationsToApply(userVersion);
        Console.WriteLine($"Migrations to apply: {migrations.Count}");

        if (migrations.Count <= 0)
        {
            return;
        }

        Console.WriteLine($"Applying {migrations.Count} migration files");
        ApplyMigrations(conn, userVersion, migrations);
    }

    private static int GetUserVersion(SqliteConnection conn)
    {
        var command = conn.CreateCommand();
        command.CommandText = @"PRAGMA user_version;";

        using var reader = command.ExecuteReader();
        reader.Read();
        var rawVersion = reader.GetString(0);
        reader.Close();
        return int.Parse(rawVersion);
    }

    private static void SetUserVersion(SqliteConnection conn, int version)
    {
        if (version < 1)
        {
            throw new InvalidOperationException("Value must be greater than zero");
        }

        var command = conn.CreateCommand();
        command.CommandText = @"PRAGMA user_version = " + version + ";";

        var resp = command.ExecuteNonQuery();
        if (resp != 0)
        {
            throw new InvalidOperationException("Unable to update user version");
        }
    }

    private static Dictionary<int, string> FindMigrationsToApply(int appliedVersion)
    {
        var migrationFiles = new Dictionary<int, string>();
        var sqlFolderFiles = Directory.GetFiles(SqlMigrationsPath);
        foreach (var filePath in sqlFolderFiles)
        {
            if (!filePath.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var fileName = filePath.Replace(SqlMigrationsPath, "");
            var versionString = fileName.Replace(".sql", "");
            if (!int.TryParse(versionString, out var version))
            {
                continue;
            }

            if (version <= appliedVersion)
            {
                continue;
            }

            migrationFiles[version] = filePath;
        }

        return migrationFiles;
    }

    private static void ApplyMigrations(SqliteConnection conn, int appliedVersion, Dictionary<int, string> filePaths)
    {
        // There are migrations left to apply
        while (filePaths.Count > 0)
        {
            appliedVersion++;

            if (!filePaths.TryGetValue(appliedVersion, out var migrationPath))
            {
                throw new InvalidOperationException($"Migration could not be found for migration {appliedVersion}");
            }

            var query = File.ReadAllText(migrationPath);
            var command = conn.CreateCommand();
            command.CommandText = query;

            var resp = command.ExecuteNonQuery();
            if (resp != 0)
            {
                Console.WriteLine($"Error applying migration {migrationPath}: {resp}");
            }

            // Update user version so we can tell the migration completed
            SetUserVersion(conn, appliedVersion);

            // Remove the migration from the dictionary to signal it no longer needs to run
            filePaths.Remove(appliedVersion);
        }
    }
}
