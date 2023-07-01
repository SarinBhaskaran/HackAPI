using Elasticsearch.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Nest;
using System.Security.Cryptography;

namespace HackApi
{
    public class ParkingLocationByRoute
    {
        public List<Source> GetParkingLocations(List<Coordinates> waypoints, string[] locationtype, bool? hasParking)
        {
            List<Source> locations = new List<Source>();
            var singleNodeConnectionPool = new SingleNodeConnectionPool(new Uri("https://xxxxxxxx"));
            var connectionSettings = new Nest.ConnectionSettings(singleNodeConnectionPool);
            connectionSettings.BasicAuthentication("xxxxxxxx", "xxxxxxxxxxxxxxxxx");
            // Create a client.
            var client = new ElasticClient(connectionSettings);

            var coordinates = waypoints.Select(o=> new GeoLocation(o.Latitude, o.Longitude)).ToList();

            ISearchResponse<Source>? searchResponse;
            // Execute the query and get the results.
            if (hasParking!=null)
            {
                if (locationtype.Any())
                {

                    searchResponse = client.Search<Source>(s => s
                        .Index("xxxxxxxxxxx")
                        .Query(q => q
                        .Bool(b => b
                        .Must(s => s.Term(t => t.Field("hasParking").Value(hasParking)),
                            s => s.Terms(c => c.Field("locationType").Terms(locationtype)))
                        .Filter(f => f
                        .GeoPolygon(g => g
                        .Field("locationGeo")
                        .Points(coordinates)
                         )
                         ))).Size(600));
                }
                else
                {


                    searchResponse = client.Search<Source>(s => s
                       .Index("xxxxxxxxxxxxxxx")
                       .Query(q => q
                       .Bool(b => b
                       .Must(s => s.Term(t => t.Field("hasParking").Value(hasParking)))
                       .Filter(f => f
                       .GeoPolygon(g => g
                           .Field("locationGeo")
                           .Points(coordinates)
                           )
                       ))).Size(600));
                }
            }
            else 
            {
                if (locationtype.Any())
                {
                    searchResponse = client.Search<Source>(s => s
                        .Index("xxxxxxxxxxxx")
                        .Query(q => q
                        .Bool(b => b
                        .Must(s => s.Terms(c => c.Field("locationType").Terms(locationtype)))
                        .Filter(f => f
                        .GeoPolygon(g => g
                        .Field("locationGeo")
                        .Points(coordinates)
                         )
                         ))).Size(600));
                }
                else
                {
                    searchResponse = client.Search<Source>(s => s
                       .Index("xxxxxxxxxxxxxxxx")
                       .Query(q => q
                       .Bool(b => b
                       .Filter(f => f
                       .GeoPolygon(g => g
                           .Field("locationGeo")
                           .Points(coordinates)
                           )
                       ))).Size(600));
                }
            }



            if (searchResponse.IsValid)
            {
                // Handle the search results
                foreach (var hit in searchResponse.Hits)
                {
                    var document = hit.Source;
                    locations.Add(document);
                    // Process each document as needed
                }
            }
            else
            {
                // Handle search error
                Console.WriteLine($"Error occurred: {searchResponse.ServerError.Error}");
            }

            return locations;
        }

    }


    public class src
    {
        public string _index { get; set; }
        public string _id { get; set; }
        public double _score { get; set; }
        public Source _source { get; set; }
        public List<string> _ignored { get; set; }
    }

    public class Source
    {
        public string locationType { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string locationGeo { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public bool hasFuelStation { get; set; }
        public bool hasCatScale { get; set; }
        public bool hasParking { get; set; }
        public bool hasWalmart { get; set; }
        public bool hasRestaurants { get; set; }
        public string amenities { get; set; }
        public int amenitiesType { get; set; }
        public int rating { get; set; }
        public string verificationStatus { get; set; }
        public int numberOfParkingSpots { get; set; }
        public int confidenceScore { get; set; }
        public string hoursOfOperation { get; set; }
        public DateTime timestamp { get; set; }
        public List<Review> reviews { get; set; }
    }

    public class Review
    {
        public string ReviewComment { get; set; }
        public DateTime? Timestamp { get; set; }

    }

    public class SearchFilter { 
        public string Origin { get; set; }
        public string Destination { get; set; }
        public bool? HasParking { get; set; } = null;
        public string[]? LocationType { get; set; } = null;

    }

}
