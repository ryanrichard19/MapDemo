using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BingMapsRESTToolkit;
using Microsoft.Extensions.Configuration;

namespace MapDemo.Services 


{
    public class BingMaps
    {
        private readonly string _ApiKey;

        public BingMaps(IConfiguration configuraton)
        {
            _ApiKey = configuraton["BingKey"];
        }

        public async Task<Location> Geocode(string query) {
            var request = new GeocodeRequest() {
                BingMapsKey = _ApiKey,
                Query = query

            };

            var resources = await GetResoucesFromRequestAsync(request);

            return resources.FirstOrDefault() as Location;
        }

        private static async Task<Resource[]> GetResoucesFromRequestAsync(BaseRestRequest request)
        {
            var response = await request.Execute();
            return response.ResourceSets[0].Resources;
        } 

    }


}