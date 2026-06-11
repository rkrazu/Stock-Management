using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Stock_Managemnet.Data
{
    public static class DatabaseBackupService
    {
        public static string ServerName =>
            new SqlConnectionStringBuilder(DatabaseInitializer.ConnectionString).DataSource;

        public static string DatabaseName =>
            new SqlConnectionStringBuilder(DatabaseInitializer.ConnectionString).InitialCatalog;

        public static void Backup(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath))
                throw new ArgumentException("Backup file path is required.", nameof(backupFilePath));

            var directory = Path.GetDirectoryName(backupFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var databaseName = DatabaseName;
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("Database name is missing from the connection string.");

            using (var connection = new SqlConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText =
                        $"BACKUP DATABASE [{EscapeIdentifier(databaseName)}] TO DISK = @path WITH INIT, COPY_ONLY, CHECKSUM, STATS = 10";
                    command.Parameters.AddWithValue("@path", backupFilePath);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void Restore(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath))
                throw new ArgumentException("Backup file path is required.", nameof(backupFilePath));

            if (!File.Exists(backupFilePath))
                throw new FileNotFoundException("Backup file was not found.", backupFilePath);

            SqlConnection.ClearAllPools();

            var databaseName = DatabaseName;
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("Database name is missing from the connection string.");

            var masterConnectionString = DatabaseInitializer.MasterConnectionString;
            var files = ReadBackupFileList(backupFilePath, masterConnectionString);
            var moveClause = BuildMoveClause(files, databaseName, masterConnectionString);

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                if (DatabaseExists(connection, databaseName))
                    SetMultiUserMode(connection, databaseName, singleUser: true);

                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 0;
                        command.CommandText =
                            $"RESTORE DATABASE [{EscapeIdentifier(databaseName)}] FROM DISK = @path WITH REPLACE, RECOVERY{moveClause}";
                        command.Parameters.AddWithValue("@path", backupFilePath);
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    if (DatabaseExists(connection, databaseName))
                        SetMultiUserMode(connection, databaseName, singleUser: false);
                }
            }
        }

        private static bool DatabaseExists(SqlConnection connection, string databaseName)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT DB_ID(@Name)";
                command.Parameters.AddWithValue("@Name", databaseName);
                return command.ExecuteScalar() != DBNull.Value;
            }
        }

        private static void SetMultiUserMode(SqlConnection connection, string databaseName, bool singleUser)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandTimeout = 0;
                command.CommandText = singleUser
                    ? $"ALTER DATABASE [{EscapeIdentifier(databaseName)}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
                    : $"ALTER DATABASE [{EscapeIdentifier(databaseName)}] SET MULTI_USER";
                command.ExecuteNonQuery();
            }
        }

        private static List<BackupFileEntry> ReadBackupFileList(string backupFilePath, string masterConnectionString)
        {
            var files = new List<BackupFileEntry>();

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "RESTORE FILELISTONLY FROM DISK = @path";
                    command.Parameters.AddWithValue("@path", backupFilePath);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            files.Add(new BackupFileEntry
                            {
                                LogicalName = reader.GetString(0),
                                PhysicalName = reader.GetString(1),
                                Type = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            if (files.Count == 0)
                throw new InvalidOperationException("The selected file is not a valid SQL Server backup.");

            return files;
        }

        private static string BuildMoveClause(
            IReadOnlyList<BackupFileEntry> files,
            string databaseName,
            string masterConnectionString)
        {
            var dataPath = GetServerProperty(masterConnectionString, "InstanceDefaultDataPath");
            var logPath = GetServerProperty(masterConnectionString, "InstanceDefaultLogPath");

            if (string.IsNullOrWhiteSpace(dataPath) || string.IsNullOrWhiteSpace(logPath))
                return string.Empty;

            var builder = new StringBuilder();
            var dataIndex = 0;

            foreach (var file in files)
            {
                string targetPath;
                if (string.Equals(file.Type, "L", StringComparison.OrdinalIgnoreCase))
                {
                    targetPath = Path.Combine(logPath, $"{databaseName}_log.ldf");
                }
                else
                {
                    dataIndex++;
                    targetPath = dataIndex == 1
                        ? Path.Combine(dataPath, $"{databaseName}.mdf")
                        : Path.Combine(dataPath, $"{databaseName}_{dataIndex}.ndf");
                }

                builder.Append(", MOVE '");
                builder.Append(file.LogicalName.Replace("'", "''"));
                builder.Append("' TO '");
                builder.Append(targetPath.Replace("'", "''"));
                builder.Append('\'');
            }

            return builder.ToString();
        }

        private static string GetServerProperty(string masterConnectionString, string propertyName)
        {
            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT CAST(SERVERPROPERTY(@Name) AS NVARCHAR(512))";
                    command.Parameters.AddWithValue("@Name", propertyName);
                    var value = command.ExecuteScalar();
                    return value == DBNull.Value ? null : Convert.ToString(value);
                }
            }
        }

        private static string EscapeIdentifier(string name) =>
            name.Replace("]", "]]");

        private sealed class BackupFileEntry
        {
            public string LogicalName { get; set; }
            public string PhysicalName { get; set; }
            public string Type { get; set; }
        }
    }
}
