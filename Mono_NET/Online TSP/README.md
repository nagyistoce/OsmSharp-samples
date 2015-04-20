# Online TSP

Demonstrates how to use OsmSharp and the [OsmSharp routing service](https://github.com/OsmSharp/OsmSharp.Service.Routing) to calculate a TSP-solution. The routing service does all the hard work, this code is a client that demonstrates how to sort a set of locations in a given CSV-file and output the same CSV-file but sorted along with the route.

INPUT: 
* CSV-file with lat-lon and other info.
OUTPUT: 
* CSV-file with lat-lon and other info BUT sorted along shortest path.
* GPX-file with the route along all points in the CSV-file.
 
To run this sample you need to setup the [OsmSharp routing service](https://github.com/OsmSharp/OsmSharp.Service.Routing) with the antwerp.osm.pbf in this folder and use the configuration file antwerp.config.