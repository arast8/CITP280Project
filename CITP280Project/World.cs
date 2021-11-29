using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITP280Project
{
    /// <summary>
    /// Represents a simulated game world.
    /// A World only supports one player.
    /// Worlds save to and read from a SQLite database which is at "Worlds/name of world.sqlite".
    /// </summary>
    public class World : IDisposable
    {
        private const int TEN_SECONDS = 10_000;
        private const int ZONE_UNLOAD_DISTANCE = 100;
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

        private Player player;
        private readonly Dictionary<Point<int>, Zone> zones = new Dictionary<Point<int>, Zone>();
        private SqliteConnection connection;
        private Timer saveTimer = new Timer { Interval = TEN_SECONDS };
        private string worldsDirName = "Worlds";
        private string fileName;

        public string Name { get; private set; }

        public World(string name)
        {
            Name = name;
            fileName = $"{worldsDirName}/{name}.sqlite";

            OpenDB();

            saveTimer.Tick += SaveTimer_Tick;
            saveTimer.Start();
        }

        /// <summary>
        /// Tries to load a Player from the database.
        /// If they do not exist yet, creates them and saves them to the database.
        /// </summary>
        /// <param name="name">The name of the Player to load/create</param>
        /// <returns>The loaded or created player</returns>
        public Player CreatePlayer(string name)
        {
            if (player != null)
                throw new Exception("A Player already exists in this world.");

            if (DBTryGetPlayer(name, out var savedPlayer))
            {
                player = savedPlayer;
            }
            else
            {
                player = new Player(name);
                DBInsertPlayer();
            }

            return player;
        }

        public void timerTick_Tick()
        {
            player.Move();
        }

        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            Save();

            UnloadFarZones();
        }

        /// <summary>
        /// Saves this World's state to its database.
        /// </summary>
        public void Save()
        {
            DBUpdateAllMaterials();
            DBUpdatePlayer();
        }

        /// <summary>
        /// Removes zones from the zone dictionary that are further than ZONE_UNLOAD_DISTANCE away from the player.
        /// </summary>
        private void UnloadFarZones()
        {
            var query = from KeyValuePair<Point<int>, Zone> pair in zones
                        where player.Location.DistanceTo(pair.Value.Center) > ZONE_UNLOAD_DISTANCE
                        select pair.Key;

            // Calling .toArray() on query avoids modifying the collection on which the query is executing.
            foreach (Point<int> location in query.ToArray())
                zones.Remove(location);
        }

        /// <summary>
        /// Gets or creates a Zone at the specified coordinates.
        /// If it does not exist in memory, tries to read it from the database.
        /// If it does not exist in the database, creates it and saves it to the database.
        /// </summary>
        /// <returns>The Zone at the specified coordinates</returns>
        public Zone GetZone(int x, int y)
        {
            int zoneX = MathHelper.Floor(x, Zone.ZoneSize);
            int zoneY = MathHelper.Floor(y, Zone.ZoneSize);

            var location = new Point<int>(zoneX, zoneY);

            if (zones.ContainsKey(location))
                return zones[location];
            else if (DBTryGetZone(location, out var savedZone))
            {
                zones[location] = savedZone;
                return savedZone;
            }
            else
            {
                var newZone = new Zone(location);
                newZone.Init();

                zones[location] = newZone;

                DBInsertZone(newZone);

                return newZone;
            }
        }

        /// <summary>
        /// Returns the Material of the ground at the specified x and y coordinates.
        /// </summary>
        public Material GetGroundMaterial(int x, int y)
        {
            var zone = GetZone(x, y);
            return zone.Ground[x - zone.Location.X, y - zone.Location.Y];
        }

        public Material GetGroundMaterial(Point<int> location) => GetGroundMaterial(location.X, location.Y);
        public Material GetGroundMaterial(float x, float y) => GetGroundMaterial(Convert.ToInt32(Math.Floor(x)), Convert.ToInt32(Math.Floor(y)));

        /// <summary>
        /// Returns the Material above the ground at the specified x and y coordinates.
        /// </summary>
        public Material GetPlayerLevelMaterial(int x, int y)
        {
            var zone = GetZone(x, y);
            return zone.PlayerLevel[x - zone.Location.X, y - zone.Location.Y];
        }

        /* Database things */

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
                DBCreateTables();
        }

        public void Dispose()
        {
            saveTimer.Stop();

            try
            {
                Save();
            }
            finally
            {
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Creates the needed tables in the database.
        /// </summary>
        private void DBCreateTables()
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
        /// Inserts the World's current player into the database.
        /// </summary>
        private void DBInsertPlayer()
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
        private void DBInsertZone(Zone zone)
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
        /// Checks all Materials in all of this World's Zones to see if they have been changed
        /// since the last save, and updates them in the database if they have been.
        /// </summary>
        private void DBUpdateAllMaterials()
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
                            if (zone.Changed[x, y])
                            {
                                command.Parameters.Clear();
                                command.Parameters.Add("@ground", SqliteType.Text).Value = zone.Ground[x, y].Name;
                                command.Parameters.Add("@playerLevel", SqliteType.Text).Value = zone.PlayerLevel[x, y].Name;
                                command.Parameters.Add("@x", SqliteType.Integer).Value = zone.Location.X + x;
                                command.Parameters.Add("@y", SqliteType.Integer).Value = zone.Location.Y + y;

                                command.ExecuteNonQuery();

                                zone.Changed[x, y] = false;
                            }
                        }
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Updates this World's player in the database.
        /// </summary>
        public void DBUpdatePlayer()
        {
            if (player != null)
            {
                using (var command = new SqliteCommand(UPDATE_PLAYER_QUERY, connection))
                {
                    command.Parameters.Add("@name", SqliteType.Text).Value = player.Name;
                    command.Parameters.Add("@x", SqliteType.Real).Value = player.Location.X;
                    command.Parameters.Add("@y", SqliteType.Real).Value = player.Location.Y;
                    command.Parameters.Add("@hunger", SqliteType.Real).Value = player.Hunger;

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
        private bool DBTryGetPlayer(string name, out Player savedPlayer)
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

                        savedPlayer = new Player(name, location, hunger);
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
        private bool DBTryGetZone(Point<int> location, out Zone savedZone)
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
    }
}
