using System;
using System.Data.SqlClient;

namespace Stock_Managemnet.Data
{
    public static class CredentialStore
    {
        public static bool Exists()
        {
            using (var connection = new SqlConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(1) FROM AppCredentials WHERE Id = 1";
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public static StoredCredential Get()
        {
            using (var connection = new SqlConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT PasswordHash, PasswordSalt FROM AppCredentials WHERE Id = 1";
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new StoredCredential
                        {
                            Hash = reader.GetString(0),
                            Salt = reader.GetString(1)
                        };
                    }
                }
            }
        }

        public static void Save(string passwordHash, string passwordSalt)
        {
            using (var connection = new SqlConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
IF EXISTS (SELECT 1 FROM AppCredentials WHERE Id = 1)
    UPDATE AppCredentials
    SET PasswordHash = @Hash, PasswordSalt = @Salt, UpdatedAt = @UpdatedAt
    WHERE Id = 1;
ELSE
    INSERT INTO AppCredentials (Id, PasswordHash, PasswordSalt, UpdatedAt)
    VALUES (1, @Hash, @Salt, @UpdatedAt);";
                    command.Parameters.AddWithValue("@Hash", passwordHash);
                    command.Parameters.AddWithValue("@Salt", passwordSalt);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        public sealed class StoredCredential
        {
            public string Hash { get; set; }
            public string Salt { get; set; }
        }
    }
}
