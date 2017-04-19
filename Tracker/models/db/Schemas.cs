using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker.models.db
{

    public class Route
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int id { get; set; }
        public long stamp { get; set; }
        public double distance { get; set; }
        public double avgSpeed { get; set; }
        public long time { get; set; }
        public string image { get; set; }
    }
    public class RoutePoints
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int id { get; set; }
        [SQLite.Indexed]
        public int id_parent { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double altitude { get; set; }
        public long stamp { get; set; }
        public double speed { get; set; }
    }

}
