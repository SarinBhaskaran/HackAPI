using GoogleApi.Entities.Maps.Common;
using Microsoft.AspNetCore.Mvc;

namespace HackApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<ParkingController> _logger;

        public ParkingController(ILogger<ParkingController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "ParkingLocations")]
        public IEnumerable<Source> Post(SearchFilter filter)
        {
            string[] LocationTypeArray = new string[] { };
            FindRoute fr = new FindRoute();
            var points = fr.GetRouteWaypoints(filter.Origin, filter.Destination);
            var geoPolygonGenerator = new GeoPolygonGenerator();
            var polygon = geoPolygonGenerator.GenerateGeoPolygon(points, 20);
            var orderedPolygon = geoPolygonGenerator.ChangeOrderForPolygon(polygon);
            var parkingLocations = new ParkingLocationByRoute();
            if (filter.LocationType!=null)
            {
                LocationTypeArray = filter.LocationType;
            }
            return parkingLocations.GetParkingLocations(orderedPolygon, LocationTypeArray, filter.HasParking);
        }

        [HttpGet(Name = "ParkingLocations")]
        public IEnumerable<Source> Get(string Origin, string Destination, string? LocationType = null, bool? HasParking = null)
        {
            string[] LocationTypeArray = new string[] { };
            FindRoute fr = new FindRoute();
            var points = fr.GetRouteWaypoints(Origin, Destination);
            var geoPolygonGenerator = new GeoPolygonGenerator();
            var polygon = geoPolygonGenerator.GenerateGeoPolygon(points, 20);
            var orderedPolygon = geoPolygonGenerator.ChangeOrderForPolygon(polygon);
            var parkingLocations = new ParkingLocationByRoute();
            if(!string.IsNullOrEmpty(LocationType))
            {
                LocationTypeArray = new string[] { LocationType }; 
            }
            return parkingLocations.GetParkingLocations(orderedPolygon, LocationTypeArray,HasParking);

        }
    }
}