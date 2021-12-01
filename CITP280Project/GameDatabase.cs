using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    class GameDatabase : IDisposable
    {
        private const string CREATE_MATERIALS_TABLE_QUERY =
            @"CREATE TABLE Materials (
                X INT,
                Y INT,
                Ground TEXT,
                PlayerLevel TEXT,
                PRIMARY KEY (X, Y))";
        private const string CREATE_PLAYERS_TABLE_QUERY =
            @"CREATE TABLE Players (
                Name TEXT,
                X NUM,
                Y NUM,
                Hunger NUM,
                PRIMARY KEY (NAME))";
        private const string CREATE_ZONES_TABLE_QUERY =
            @"CREATE TABLE Zones (
                X INT,
                Y INT,
                Biome TEXT,
                PRIMARY KEY (X, Y))";
        private const string INSERT_MATERIAL_QUERY =
            @"INSERT INTO Materials (X, Y, Ground, PlayerLevel)
            Values(@x, @y, @ground, @playerLevel)";
        private const string INSERT_PLAYER_QUERY =
            @"INSERT INTO Players (X, Y, Name, Hunger)
            VALUES(@x, @y, @name, @hunger)";
        private const string INSERT_ZONE_QUERY =
            @"INSERT INTO Zones (X, Y, Biome)
            Values(@x, @y, @biome)";
        private const string UPDATE_MATERIAL_QUERY =
            @"UPDATE Materials
            SET Ground = @ground,
                PlayerLevel = @playerLevel
            WHERE X == @x AND Y == @y";
        private const string UPDATE_PLAYER_QUERY =
            @"UPDATE Players
            SET X = @x, Y = @y, Hunger = @hunger
            WHERE Name = @name";
        private const string SELECT_MATERIALS_QUERY =
            @"SELECT *
            FROM Materials
            WHERE X > @xMin AND x < @xMax
                AND y > @yMin AND Y < @yMax";
        private const string SELECT_PLAYER_QUERY =
            @"SELECT *
            FROM Players
            WHERE Name = @name";
        private const string SELECT_ZONE_QUERY =
            @"SELECT *
            FROM Zones
            WHERE X = @x AND Y = @y";

        private SqliteConnection connection;
        private string worldsDirName = "Worlds";
        private string fileName;

        public GameDatabase(string worldName)
        {
            fileName = $"{worldsDirName}/{worldName}.sqlite";

            OpenDB();
        }


        /// <summary>
        /// Opens the SQLite database for this world for read/write, creating the parent directory,
        /// the database, and the tables if they do not exist.
        /// The database file will be at "Worlds/name of world.sqlite".
        /// </summary>
        private void OpenDB()
        {
            if (!Directory.Exists(worldsDirName))
                Directory.CreateDirectory(worldsDirName);
            var dbIsPreexisting = File.Exists(fileName);

            var csb = new SqliteConnectionStringBuilder
            {
                DataSource = fileName,
                Mode = SqliteOpenMode.ReadWriteCreate
            };

            connection = new SqliteConnection(csb.ToString());
            connection.Open();

            if (!dbIsPreexisting)
                CreateTables();
        }

        /// <summary>
        /// Creates the needed tables in the database.
        /// </summary>
        private void CreateTables()
        {
            using (var transaction = connection.BeginTransaction())
            using (var command = new SqliteCommand() { Connection = connection, Transaction = transaction })
            {
                command.CommandText = CREATE_MATERIALS_TABLE_QUERY;
                command.ExecuteNonQuery();

                command.CommandText = CREATE_PLAYERS_TABLE_QUERY;
                command.ExecuteNonQuery();

                command.CommandText = CREATE_ZONES_TABLE_QUERY;
                command.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        /// <summary>
        /// Inserts a player into the database.
        /// </summary>
        public void InsertPlayer(Player player)
        {
            if (player != null)
            {
                using (var command = new SqliteCommand(INSERT_PLAYER_QUERY, connection))
                {
                    command.Parameters.Add("@x", SqliteType.Integer).Value = player.Location.X;
                    command.Parameters.Add("@y", SqliteType.Integer).Value = player.Location.Y;
                    command.Parameters.Add("@name", SqliteType.Integer).Value = player.Name;
                    command.Parameters.Add("@hunger", SqliteType.Integer).Value = player.Hunger;

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a Zone into the database.
        /// </summary>
        /// <param name="zone">The Zone to insert</param>
        public void InsertZone(Zone zone)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = new SqliteCommand(INSERT_ZONE_QUERY, connection, transaction))
                {
                    command.Parameters.Add("@x", SqliteType.Integer).Value = zone.Location.X;
                    command.Parameters.Add("@y", SqliteType.Integer).Value = zone.Location.Y;
                    command.Parameters.Add("@biome", SqliteType.Text).Value = Enum.GetName(typeof(Biome), zone.Biome);

                    command.ExecuteNonQuery();
                }

                using (var command = new SqliteCommand(INSERT_MATERIAL_QUERY, connection, transaction))
                {
                    for (int x = 0; x < Zone.ZoneSize; x++)
                    {
                        for (int y = 0; y < Zone.ZoneSize; y++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add("@x", SqliteType.Integer).Value = zone.Location.X + x;
                            command.Parameters.Add("@y", SqliteType.Integer).Value = zone.Location.Y + y;
                            command.Parameters.Add("@ground", SqliteType.Text).Value = zone.Ground[x, y]?.Name;
                            string playerLevelName = zone.PlayerLevel[x, y]?.Name ?? null;

                            if (playerLevelName == null)
                                command.Parameters.Add("@playerLevel", SqliteType.Text).Value = DBNull.Value;
                            else
                                command.Parameters.Add("@playerLevel", SqliteType.Text).Value = playerLevelName;

                            command.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Checks all Materials in all Zones in zones to see if they have been changed
        /// since the last save, and updates them in the database if they have been.
        /// </summary>
        public void UpdateAllMaterials(ConcurrentDictionary<Point<int>, Zone> zones)
        {
            using (var transaction = connection.BeginTransaction())
            using (var command = new SqliteCommand(UPDATE_MATERIAL_QUERY, connection, transaction))
            {
                foreach (Zone zone in zones.Values)
                {
                    for (int x = 0; x < Zone.ZoneSize; x++)
                    {
                        for (int y = 0; y < Zone.ZoneSize; y++)
                        {
                            if (zone.IsSaved[x, y])
                            {
                                command.Parameters.Clear();
                                command.Parameters.Add("@ground", SqliteType.Text).Value = zone.Ground[x, y].Name;
                                command.Parameters.Add("@playerLevel", SqliteType.Text).Value = zone.PlayerLevel[x, y].Name;
                                command.Parameters.Add("@x", SqliteType.Integer).Value = zone.Location.X + x;
                                command.Parameters.Add("@y", SqliteType.Integer).Value = zone.Location.Y + y;

                                zone.IsSaved[x, y] = true;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Updates a player in the database.
        /// </summary>
        public void UpdatePlayer(Player player)
        {
            if (player != null && !player.IsSaved)
            {

                using (var command = new SqliteCommand(UPDATE_PLAYER_QUERY, connection))
                {
                    lock (player.StateChangeLock)
                    {
                        command.Parameters.Add("@name", SqliteType.Text).Value = player.Name;
                        command.Parameters.Add("@x", SqliteType.Real).Value = player.Location.X;
                        command.Parameters.Add("@y", SqliteType.Real).Value = player.Location.Y;
                        command.Parameters.Add("@hunger", SqliteType.Real).Value = player.Hunger;
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Tries to read a Player from the database.
        /// </summary>
        /// <param name="name">The name of the Player to retrieve</param>
        /// <param name="savedPlayer">The Player is passed out through this parameter.
        /// It will be null if the database does not have the Player.</param>
        /// <returns>Whether the database has the Player</returns>
        /// <remarks>This method does not handle errors.</remarks>
        public bool TryGetPlayer(string name, out Player savedPlayer)
        {
            using (var command = new SqliteCommand(SELECT_PLAYER_QUERY, connection))
            {
                command.Parameters.Add("@name", SqliteType.Text).Value = name;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var location = new Point<double>(
                            reader.GetDouble(reader.GetOrdinal("X")),
                            reader.GetDouble(reader.GetOrdinal("Y"))
                        );
                        var hunger = reader.GetDouble(reader.GetOrdinal("Hunger"));

                        savedPlayer = new Player(name, location, hunger, true);
                        return true;
                    }
                    else
                    {
                        savedPlayer = null;
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Tries to read a Zone and all of its Materials from the database.
        /// </summary>
        /// <param name="location">The location of the Zone to get</param>
        /// <param name="savedZone">The Zone is passed out through this parameter.
        /// It will be null if the database does not have the Zone.</param>
        /// <returns>Whether the database has the Zone</returns>
        /// <remarks>This method does not handle errors.</remarks>
        public bool TryGetZone(Point<int> location, out Zone savedZone)
        {
            bool dbHasZone;
            string biomeName = "";

            using (var command = new SqliteCommand() { Connection = connection })
            {
                command.CommandText = SELECT_ZONE_QUERY;

                command.Parameters.Add("@x", SqliteType.Integer).Value = location.X;
                command.Parameters.Add("@y", SqliteType.Integer).Value = location.Y;

                using (var reader = command.ExecuteReader())
                {
                    dbHasZone = reader.Read();

                    if (dbHasZone)
                        biomeName = reader.GetString(reader.GetOrdinal("Biome"));
                }

                if (dbHasZone)
                {
                    savedZone = new Zone(location);

                    if (Enum.TryParse(biomeName, out Biome biome))
                        savedZone.Biome = biome;
                    else
                        savedZone.Biome = Biome.Unknown;

                    command.CommandText = SELECT_MATERIALS_QUERY;

                    command.Parameters.Add("@xMin", SqliteType.Integer).Value = location.X - 1;
                    command.Parameters.Add("@xMax", SqliteType.Integer).Value = location.X + Zone.ZoneSize;
                    command.Parameters.Add("@yMin", SqliteType.Integer).Value = location.Y - 1;
                    command.Parameters.Add("@yMax", SqliteType.Integer).Value = location.Y + Zone.ZoneSize;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int absX = reader.GetInt32(reader.GetOrdinal("X"));
                            int relX = absX - savedZone.Location.X;
                            int absY = reader.GetInt32(reader.GetOrdinal("Y"));
                            int relY = absY - savedZone.Location.Y;
                            string ground = reader.GetString(reader.GetOrdinal("Ground"));

                            string playerLevel =
                                reader.IsDBNull(reader.GetOrdinal("PlayerLevel"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PlayerLevel"));

                            savedZone.Ground[relX, relY] = Material.GetMaterial(ground);
                            savedZone.PlayerLevel[relX, relY] = Material.GetMaterial(playerLevel);
                        }
                    }

                    return true;
                }
                else
                {
                    savedZone = null;
                    return false;
                }
            }
        }

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}
