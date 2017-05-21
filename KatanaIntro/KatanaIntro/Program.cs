using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatanaIntro
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:8080"; // listen on local machine port 8080
            
            // Tells Katana to start server using configuration in Startup object using uri
            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started!");
                Console.ReadKey();
                Console.WriteLine("Stopping!");
            }
        }
    }
    public class Startup
    {
        // IAppBuilder contains method to configure how we want to handle requests and responses
        public void Configuration(IAppBuilder app)
        {
            app.Run(ctx =>
            {
                return ctx.Response.WriteAsync("Hello World!"); // WriteAsyc() returns a task
                                                                // for every http request, print hello world
            });
        }
    }
}
