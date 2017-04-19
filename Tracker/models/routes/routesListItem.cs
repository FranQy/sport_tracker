using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Tracker.models.routes
{
    class routesListItem
    {

        public int id { get; set; }
        public string stamp { get; set; }
        public BitmapImage image { get; set; }
        public string distance { get; set; }
        public string time { get; set; }
        public string speed { get; set; }
    }
}
