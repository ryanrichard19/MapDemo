using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MapDemo.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BingMapsRESTToolkit;

namespace MapDemo
{
    public class DummyDataHub : Hub
    {



    }

    public class DummyDataService : IHostedService
    {
        private readonly IServiceProvider _Services;
        private readonly BingMaps _Maps;
        private CancellationToken _Token;
        private Task _BackgroundTask;

        public DummyDataService(IServiceProvider services, BingMaps maps)
        {
            _Services = services;
            _Maps = maps;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _Token = cancellationToken;
            _BackgroundTask = Run();
            return Task.CompletedTask;
        }

        public async Task Run()
        {

            while (!_Token.IsCancellationRequested)
            {
                var hub = _Services.GetRequiredService<IHubContext<DummyDataHub>>();
                var index = new Random();
                var thisCity = Cities[index.Next(0,6)];
                //var thisCity = Cities[0];
                if (thisCity.Item2 == null)
                {
                    thisCity.Item2 = (await _Maps.Geocode(thisCity.Item1)).GeocodePoints[0].Coordinates;
                }

                try
                {
                    Console.WriteLine("Sending Infomation " + thisCity);
                    await hub?.Clients?.All?.SendAsync("NewSale", thisCity.Item1, thisCity.Item2[0], thisCity.Item2[1]);
                }
                catch (Exception e)
                {

                    Console.WriteLine("Error " + e.Message);
                }



                await Task.Delay(TimeSpan.FromSeconds(5));
            }



        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private List<(string, double[])> Cities = new List<(string, double[])> {
            {("KwaZulu-Natal Durban", null)},
            {("KwaZulu-Natal Pietermaritzburg", null)},
            {("KwaZulu-Natal Empangeni", null)},
            {("Western Cape Oudtshoorn", null)},
            {("Free State Bloemfontein", null)},
            {("Gauteng Johannesburg", null)},
            {("Western Cape Cape Town", null)}
        };
    }
}