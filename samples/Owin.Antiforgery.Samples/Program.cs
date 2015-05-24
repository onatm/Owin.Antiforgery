using Microsoft.Owin.Hosting;
using System;

namespace Owin.Antiforgery.Samples
{
    class Program
    {
        static void Main()
        {
            const string uri = "http://localhost:8080/";

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started");
                Console.ReadKey();
                Console.WriteLine("Stopping");
            }
        }
    }
}
