using Microsoft.AspNetCore.Mvc;

namespace HackApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {


        private readonly ILogger<ParkingController> _logger;

        public PingController(ILogger<ParkingController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "ParkingLocationsRoute")]
        public IEnumerable<Source> Post(Route[] root)
        {
            FindRoute fr = new FindRoute();
            var waypoints = fr.GetWaypoints(root.ToList());

            var parkingLocations = new ParkingLocationByRoute();
            return parkingLocations.GetParkingLocations(waypoints, new string[0], null);
        }

        [HttpGet(Name = "Ping")]
        public string Get()
        {
            var ping = new System.Net.NetworkInformation.Ping();

            var result = ping.Send("https://nxo-gp-dev.es.us-central1.gcp.cloud.es.io");

            if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
                return "Fail";
            else
                return "Success";

        }
    }
}