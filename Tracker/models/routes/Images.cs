using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tracker.models.db;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace Tracker.models.routes
{
    public class Images
    {
        private HttpClient _webClient;
        private string _url = "http://maps.googleapis.com/maps/api/staticmap?size=";
        private double _scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        public Images(HttpClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<string> download(List<RoutePoints> points)
        {
            string url = await this.getUrl(points);

            //byte[] buffer = _webClient.GetByteArrayAsync(url).Result; // Download file


            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("image.png", CreationCollisionOption.GenerateUniqueName); // Create local file
            //    //   StorageFile file = await KnownFolders.PicturesLibrary.CreateFileAsync("image.png", CreationCollisionOption.GenerateUniqueName); // Create local file

            
            //Stream stream = await file.OpenStreamForWriteAsync();
            //    stream.Write(buffer, 0, buffer.Length); // Save



            byte[] responseBytes = _webClient.GetByteArrayAsync(url).Result;

            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);

            var outputStream = stream.GetOutputStreamAt(0);

            DataWriter writer = new DataWriter(outputStream);
            writer.WriteBytes(responseBytes);
            await writer.StoreAsync();
            await outputStream.FlushAsync();


            return file.Name;
        }

        public async Task<string> getUrl(List<RoutePoints> points)
        {
            if (points.Count == 0)
            {
                return "";
            }
            string url = this.getUrlWithRes();

            foreach (RoutePoints point in points)
            {
                url += point.latitude.ToString() + "," + point.longitude.ToString() + '|';
            }
            url = url.Remove(url.Length - 1);

            return url;
        }

        private string getUrlWithRes()
        {
            var width = Window.Current.Bounds.Width * _scaleFactor;
            return _url + width.ToString() + "x" + (width / 2).ToString() + "&path=";


        }



    }
}
