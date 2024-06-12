using System;
using System.IO;

namespace DataBaseConnector
{
    public class SettingsManager
    {
        private string filePath;

        public SettingsManager(string filePath)
        {
            this.filePath = filePath;
        }

        public bool ExistsSettingsFile()
        {
            return File.Exists(filePath);
        }

        public void SaveSettings(string dbPath, string username, string password, bool isPostgreSQL, string database, string table)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(dbPath);
                writer.WriteLine(username);
                writer.WriteLine(password);
                writer.WriteLine(isPostgreSQL ? "PostgreSQL" : "Access");
                writer.WriteLine(database);
                writer.WriteLine(table);
            }
        }

        public (string dbPath, string username, string password, bool isPostgreSQL, string database, string table) LoadSettings()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Settings file not found.");

            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length < 6)
                throw new InvalidOperationException("Settings file is corrupt.");

            string dbPath = lines[0];
            string username = lines[1];
            string password = lines[2];
            bool isPostgreSQL = lines[3] == "PostgreSQL";
            string database = lines[4];
            string table = lines[5];

            return (dbPath, username, password, isPostgreSQL, database, table);
        }
    }
}
