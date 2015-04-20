using Newtonsoft.Json;
using OsmSharp.Routing;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TSPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var vehicle = "bicycle"; // the vehicle type to calculate for.
            var instance = "antwerp"; // the instance name on the routing service.

            // Read the CSV-data.
            var lines = OsmSharp.IO.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new FileInfo("bicylestations.csv").OpenRead(), OsmSharp.IO.DelimitedFiles.DelimiterType.DotCommaSeperated, true);

            // Build the request.
            var query = "/routing?";
            query = query + "vehicle=" + vehicle;
            for(int idx = 0; idx < lines.Length; idx++)
            {
                var line = lines[idx];
                // parse first.
                var lat = double.Parse(line[0], System.Globalization.CultureInfo.InvariantCulture);
                var lon = double.Parse(line[1], System.Globalization.CultureInfo.InvariantCulture);
                query = query + "&loc=" + lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + lon.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            query = query + "&sort=true&format=osmsharp"; // make sure the service does the TSP-calculation.

            // Read the response.
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(
                "http://localhost:1234/" + instance + query);
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            var response = httpWebRequest.GetResponse(); // get the data.
            Route route = null;
            using (var responseStream = new StreamReader(response.GetResponseStream()))
            {
                var responseString = responseStream.ReadToEnd();
                route = JsonConvert.DeserializeObject<Route>(responseString);
            }


            // Write output CSV-data.
            var points = new List<int>();
            foreach(var segment in route.Segments)
            {
                if (segment.Points == null) { continue; }
                foreach(var point in segment.Points)
                {
                    if (point.Tags == null) { continue; }
                    foreach(var tag in point.Tags)
                    {
                        if(tag.Key == "point_id")
                        {
                            points.Add(int.Parse(tag.Value));
                            break;
                        }
                    }
                }
            }
            var sortedLines = new string[points.Count][];
            for(int idx = 0; idx < points.Count; idx++)
            {
                sortedLines[idx] = lines[points[idx]];
            }
            using(var csvStream = new StreamWriter((new FileInfo("output.csv").OpenWrite())))
            {
                OsmSharp.IO.DelimitedFiles.DelimitedFileHandler.WriteDelimitedFile(null, sortedLines, 
                    csvStream, OsmSharp.IO.DelimitedFiles.DelimiterType.DotCommaSeperated);
            }

            // Write GPX-data.
            using(var gpxStream = (new FileInfo("output.gpx").OpenWrite()))
            {
                route.SaveAsGpx(gpxStream);
            }
        }
    }
}
