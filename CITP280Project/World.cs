using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

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
        private const int ZONE_LOAD_DISTANCE = Zone.ZoneSize * 3;
        private const int ZONE_UNLOAD_DISTANCE = Zone.ZoneSize * 10;

        private Player player;
        private readonly ConcurrentDictionary<Point<int>, Zone> zones = new ConcurrentDictionary<Point<int>, Zone>();
        private readonly ConcurrentQueue<Point<int>> zonesToLoad = new ConcurrentQueue<Point<int>>();
        private readonly Timer saveTimer = new Timer { Interval = TEN_SECONDS };
        private readonly GameDatabase db;
        private Thread dbThread;

        public string Name { get; private set; }

        public World(string name)
        {
            Name = name;

            db = new GameDatabase(name);

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

            if (db.TryGetPlayer(name, out var savedPlayer))
            {
                player = savedPlayer;
            }
            else
            {
                player = new Player(name);
                db.InsertPlayer(player);
            }

            QueueZonesNearPlayer();

            StartLoadingZonesOnDBThread();

            return player;
        }

        // Is called when the timer in GameWindow ticks.
        public void timerTick_Tick()
        {
            player.Move();

            QueueZonesNearPlayer();

            StartLoadingZonesOnDBThread();
        }

        /// <summary>
        /// Searches Zone locations near the player, and queues locations to be
        /// loaded if they are not already loaded or queued.
        /// </summary>
        private void QueueZonesNearPlayer()
        {
            Point<int> lowerBound = new Point<int>(
                MathHelper.Floor(player.Location.X - ZONE_LOAD_DISTANCE, Zone.ZoneSize),
                MathHelper.Floor(player.Location.Y - ZONE_LOAD_DISTANCE, Zone.ZoneSize));
            Point<int> upperBound = new Point<int>(
                MathHelper.Floor(player.Location.X + ZONE_LOAD_DISTANCE, Zone.ZoneSize),
                MathHelper.Floor(player.Location.Y + ZONE_LOAD_DISTANCE, Zone.ZoneSize));

            int x, y;
            Point<int> testLocation;

            for (x = lowerBound.X; x <= upperBound.X; x += Zone.ZoneSize)
            {
                for (y = lowerBound.Y; y <= upperBound.Y; y += Zone.ZoneSize)
                {
                    testLocation = new Point<int>(x, y);

                    if (!zones.ContainsKey(testLocation) && !zonesToLoad.Contains(testLocation))
                        zonesToLoad.Enqueue(testLocation);
                }
            }
        }

        /// <summary>
        /// Starts dbThread executing <see cref="LoadQueuedZones()"/>
        /// </summary>
        private void StartLoadingZonesOnDBThread()
        {
            if (zonesToLoad.Count > 0 && (dbThread == null || !dbThread.IsAlive))
            {
                dbThread = new Thread(LoadQueuedZones) { Name = "Database Thread" };

                dbThread.Start();
            }
        }

        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            StartSavingOnDBThread();

            UnloadFarZones();
        }

        /// <summary>
        /// Starts dbThread executing <see cref="Save()"/>
        /// </summary>
        private void StartSavingOnDBThread()
        {
            if (dbThread == null || !dbThread.IsAlive)
            {
                dbThread = new Thread(Save) { Name = "Database Thread" };

                dbThread.Start();
            }
        }

        /// <summary>
        /// Removes zones from the zone dictionary that are further than ZONE_UNLOAD_DISTANCE away from the player.
        /// </summary>
        private void UnloadFarZones()
        {
            var query = from KeyValuePair<Point<int>, Zone> pair in zones
                        where player.Location.DistanceTo(pair.Value.Center) > ZONE_UNLOAD_DISTANCE
                        select pair.Key;

            foreach (Point<int> location in query)
                zones.TryRemove(location, out var _);
        }

        /// <summary>
        /// Tries to get the zone at a location. Returns true and sets zone if the Zone is loaded.
        /// Otherwise, returns false and queues it to be loaded.
        /// </summary>
        private bool TryGetZone(int x, int y, out Zone zone)
        {
            int zoneX = MathHelper.Floor(x, Zone.ZoneSize);
            int zoneY = MathHelper.Floor(y, Zone.ZoneSize);

            var location = new Point<int>(zoneX, zoneY);

            if (zones.ContainsKey(location))
            {
                zone = zones[location];
                return true;
            }
            else if (!zonesToLoad.Contains(location))
            {
                zonesToLoad.Enqueue(location);
            }

            zone = null;
            return false;
        }

        /// <summary>
        /// Returns the Material of the ground at the specified x and y coordinates.
        /// </summary>
        public Material GetGroundMaterial(int x, int y)
        {
            if (TryGetZone(x, y, out var zone))
                return zone.Ground[x - zone.Location.X, y - zone.Location.Y];
            else
                return null;
        }
        public Material GetGroundMaterial(Point<double> location) =>
            GetGroundMaterial(Convert.ToInt32(Math.Floor(location.X)), Convert.ToInt32(Math.Floor(location.Y)));

        /// <summary>
        /// Returns the Material above the ground at the specified x and y coordinates.
        /// </summary>
        public Material GetPlayerLevelMaterial(int x, int y)
        {
            if (TryGetZone(x, y, out var zone))
                return zone.PlayerLevel[x - zone.Location.X, y - zone.Location.Y];
            else
                return null;
        }
        public Material GetPlayerLevelMaterial(Point<double> location) =>
            GetPlayerLevelMaterial(Convert.ToInt32(Math.Floor(location.X)), Convert.ToInt32(Math.Floor(location.Y)));

        /// <summary>
        /// Sets the ground Material at the specified x and y coordinates.
        /// </summary>
        /// <returns>Whether the ground Material was set</returns>
        public bool SetGroundMaterial(int x, int y, Material material)
        {
            if (TryGetZone(x, y, out var zone) && (material == null || material.CanBeGround))
            {
                var zoneX = x - zone.Location.X;
                var zoneY = y - zone.Location.Y;

                zone.Ground[zoneX, zoneY] = material;
                zone.IsSaved[zoneX, zoneY] = false;

                return true;
            }

            return false;
        }
        public bool SetGroundMaterial(Point<double> location, Material material) =>
            SetGroundMaterial(Convert.ToInt32(Math.Floor(location.X)), Convert.ToInt32(Math.Floor(location.Y)), material);

        /// <summary>
        /// Sets the player level Material at the specified x and y coordinates.
        /// </summary>
        /// <returns>Whether the player level Material was set</returns>
        public bool SetPlayerLevelMaterial(int x, int y, Material material)
        {
            if (TryGetZone(x, y, out var zone) && (material == null || material.CanBePlayerLevel))
            {
                var zoneX = x - zone.Location.X;
                var zoneY = y - zone.Location.Y;

                zone.PlayerLevel[zoneX, zoneY] = material;
                zone.IsSaved[zoneX, zoneY] = false;

                return true;
            }

            return false;
        }
        public bool SetPlayerLevelMaterial(Point<double> location, Material material) =>
            SetPlayerLevelMaterial(Convert.ToInt32(Math.Floor(location.X)), Convert.ToInt32(Math.Floor(location.Y)), material);

        public void Dispose()
        {
            saveTimer.Stop();

            try
            {
                StartSavingOnDBThread();
                dbThread.Join();
            }
            finally
            {
                db?.Dispose();
            }
        }

        /* Things done on separate thread */

        /// <summary>
        /// For each Zone location in the zonesToLoad queue, tries to read the
        /// Zone from the database, and creates it if it does not exist yet.
        /// </summary>
        private void LoadQueuedZones()
        {
            while (zonesToLoad.TryDequeue(out var location))
            {
                if (db.TryGetZone(location, out var savedZone))
                {
                    zones[location] = savedZone;
                }
                else
                {
                    var newZone = new Zone(location);
                    newZone.Init();

                    zones[location] = newZone;

                    db.InsertZone(newZone);
                }
            }
        }

        /// <summary>
        /// Saves this World's state to its database.
        /// </summary>
        private void Save()
        {
            db.UpdateAllMaterials(zones);
            db.UpdatePlayer(player);
        }
    }
}
