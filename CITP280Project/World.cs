using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Represents a simulated game world.
    /// </summary>
    public class World
    {
        private Player player;
        private readonly Dictionary<Point, Zone> zones = new Dictionary<Point, Zone>();

        public World()
        { }

        public Player CreatePlayer(string name)
        {
            if (player != null)
                throw new Exception("A Player already exists in this world.");

            player = new Player(name);
            return player;
        }

        public void timerTick_Tick()
        {
            player.Move();
        }

        public Zone GetZone(int x, int y)
        {
            int zoneX = MathHelper.Floor(x, Zone.ZoneSize);
            int zoneY = MathHelper.Floor(y, Zone.ZoneSize);

            var location = new Point(zoneX, zoneY);

            if (zones.ContainsKey(location))
                return zones[location];
            else
            {
                var newZone = new Zone(location);
                zones[location] = newZone;
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

        public Material GetGroundMaterial(Point location) => GetGroundMaterial(location.X, location.Y);
        public Material GetGroundMaterial(float x, float y) => GetGroundMaterial(Convert.ToInt32(Math.Floor(x)), Convert.ToInt32(Math.Floor(y)));

        /// <summary>
        /// Returns the Material above the ground at the specified x and y coordinates.
        /// </summary>
        public Material GetPlayerLevelMaterial(int x, int y)
        {
            var zone = GetZone(x, y);
            return zone.PlayerLevel[x - zone.Location.X, y - zone.Location.Y];
        }
    }
}
