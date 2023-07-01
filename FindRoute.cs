using GoogleApi.Entities.Common;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;

namespace HackApi
{
    public class FindRoute
    {
        public List<Coordinates> GetRouteWaypoints(string Origin, string Destination)
        {
            List<Coordinates> coordinates = new List<Coordinates>();
                
            var request = new DirectionsRequest
            {
                
                Origin = new LocationEx(new Address(Origin)),
                Destination = new LocationEx(new Address(Destination)),
                Key = "AIzaSyC1jbq63oef-zTmP5otJix3hP5-ISCiBgM"

            }; 

            var response = GoogleApi.GoogleMaps.Directions.QueryAsync(request).Result;

            var steps = response.Routes.First().Legs.First().Steps;

            foreach (var step in steps.ToList())
            {
                coordinates.Add(new Coordinates(step.StartLocation.Latitude, step.StartLocation.Longitude));
                coordinates.Add(new Coordinates(step.EndLocation.Latitude, step.EndLocation.Longitude));
            }
            List<Coordinates> distinctCoordinates = coordinates
                                .GroupBy(p => new { p.Latitude, p.Longitude })
                                .Select(g => g.First())
                                .ToList();
            var xxx = Newtonsoft.Json.JsonConvert.SerializeObject(distinctCoordinates);
            return distinctCoordinates;

        }
        public List<Coordinates> GetWaypoints(List<Route> model) 
        { 
            List<Coordinates> coordinates = new List<Coordinates>();
            foreach (var modelItem in model)
            {
                var legs = modelItem.legs.FirstOrDefault();

                if (legs != null)
                {
                    coordinates.Add(new Coordinates( legs.start_location.lat, legs.start_location.lng));
                    coordinates.Add(new Coordinates(legs.end_location.lat, legs.end_location.lng));

                    foreach (var step in legs.steps)
                    {
                        coordinates.Add(new Coordinates(step.start_location.lat, step.start_location.lng));
                        coordinates.Add(new Coordinates(step.end_location.lat, step.end_location.lng));
                    }
                }
            }

            List<Coordinates> distinctCoordinates = coordinates
                                            .GroupBy(p => new { p.Latitude, p.Longitude })
                                            .Select(g => g.First())
                                            .ToList();
            var xxx = Newtonsoft.Json.JsonConvert.SerializeObject(distinctCoordinates);
            return distinctCoordinates;
        }


    }

    public class Coordinates
    {
        public Coordinates(double lat, double lng)
        {
            Latitude = lat; 
            Longitude = lng;    
        }

        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class EndLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Leg
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string end_address { get; set; }
        public EndLocation end_location { get; set; }
        public string start_address { get; set; }
        public StartLocation start_location { get; set; }
        public List<Step> steps { get; set; }
        public List<object> traffic_speed_entry { get; set; }
        public List<object> via_waypoint { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class OverviewPolyline
    {
        public string points { get; set; }
    }

    public class Polyline
    {
        public string points { get; set; }
    }

    public class Route
    {
        public Bounds bounds { get; set; }
        public string copyrights { get; set; }
        public List<Leg> legs { get; set; }
        public OverviewPolyline overview_polyline { get; set; }
        public string summary { get; set; }
        public List<object> warnings { get; set; }
        public List<object> waypoint_order { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class StartLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Step
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public EndLocation end_location { get; set; }
        public string html_instructions { get; set; }
        public Polyline polyline { get; set; }
        public StartLocation start_location { get; set; }
        public string travel_mode { get; set; }
        public string? maneuver { get; set; }
    }



    public class GeoPolygonGenerator
    {
        public List<Coordinates> GenerateGeoPolygon(List<Coordinates> polyline, double distance)
        {
            List<Coordinates> geoPolygon = new List<Coordinates>();

            for (int i = 0; i < polyline.Count; i++)
            {
                int prevIndex = (i == 0) ? polyline.Count - 1 : i - 1;
                int nextIndex = (i == polyline.Count - 1) ? 0 : i + 1;

                Coordinates prevPoint = polyline[prevIndex];
                Coordinates currentPoint = polyline[i];
                Coordinates nextPoint = polyline[nextIndex];

                // Calculate the bearing from the current point to the previous and next points
                double bearingToPrev = CalculateBearing(currentPoint, prevPoint);
                double bearingToNext = CalculateBearing(currentPoint, nextPoint);

                // Calculate the vertices of the polygon based on the distance and bearings
                Coordinates vertex1 = CalculateDestinationPoint(currentPoint, bearingToPrev - 90, distance);
                Coordinates vertex2 = CalculateDestinationPoint(currentPoint, bearingToNext + 90, distance);

                // Add the vertices to the polygon
                geoPolygon.Add(vertex1);
                geoPolygon.Add(vertex2);
            }

            return geoPolygon;
        }

        public List<Coordinates> ChangeOrderForPolygon(List<Coordinates> polygon)
        {
            var arrPolygon = polygon.ToArray();
            var newResult = new List<Coordinates>();
            int len = arrPolygon.Length;
            for (int i = 0; i < len; i++)
            {
                if (i % 2 == 0)
                    newResult.Add(arrPolygon[i]);    

            }

            int start = len;
            if (len % 2 == 0)
            {
                start = len - 1;
            }
            for (int j = start; j > 0; j--)
            {
                if (j % 2 != 0)
                    newResult.Add(arrPolygon[j]);

            }

            return newResult;
        }

        private double CalculateBearing(Coordinates point1, Coordinates point2)
        {
            double lat1 = ToRadians(point1.Latitude);
            double lon1 = ToRadians(point1.Longitude);
            double lat2 = ToRadians(point2.Latitude);
            double lon2 = ToRadians(point2.Longitude);

            double dLon = lon2 - lon1;

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double bearing = Math.Atan2(y, x);

            return ToDegrees(bearing);
        }

        private Coordinates CalculateDestinationPoint(Coordinates point, double bearing, double distance)
        {
            double lat1 = ToRadians(point.Latitude);
            double lon1 = ToRadians(point.Longitude);
            double angularDistance = distance / 3958.8; // Earth's radius in meters

            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(angularDistance) +
                Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(ToRadians(bearing)));

            double lon2 = lon1 + Math.Atan2(Math.Sin(ToRadians(bearing)) * Math.Sin(angularDistance) * Math.Cos(lat1),
                Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(lat2));

            lon2 = (lon2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI; // Normalize the longitude value

            return new Coordinates(ToDegrees(lat2), ToDegrees(lon2));
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
    }


}
