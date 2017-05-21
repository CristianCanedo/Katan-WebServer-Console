using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatanaIntro
{
    // Func delegate returns a task and essentially passes component to component
    // as request progresses through pipeline
    using AppFunc = Func<IDictionary<string, object>, Task>;

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
            // Middleware
            app.Use(async (environment, next) =>
            {
                // Incoming request traveling through pipeline being processed.
                // Writing the environment request path to the server console
                Console.WriteLine("Requesting : " + environment.Request.Path);

                await next();

                // Response coming back through pipeline.
                // Writing the environment response status code to the server console
                Console.WriteLine("Response: " + environment.Response.StatusCode);


            });
            
            ConfigureWebApi(app);
            
            app.UseHelloWorld();
        }
        
        private void ConfigureWebApi(IAppBuilder app)
        {
            // Instance of HttpConfiguration controls routing rules, serializers and routes.
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            app.UseWebApi(config);
        }
    }
    // Creating low level extension class with extension method of IAppBuilder
    public static class AppBuilderExtensions
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }
    
    public class HelloWorldComponent
    {
        // Saves next AppFunc component in private _next
        AppFunc _next;

        // Needs constructor to take one argument to process next component in OWIN pipeline
        public HelloWorldComponent(AppFunc next)
        {
            _next = next; // needs validation
        }

        // Method to match AppFunc signature and proccess components in pipeline
        public Task Invoke(IDictionary<string, object> environment)
        {
            // Reference to owin.ResponseBody to allow us to write to the Stream
            var response = environment["owin.ResponseBody"] as Stream;

            // creating new StreamWriter to allow us to write to the response body
            using(var writer = new StreamWriter(response))
            {
                return writer.WriteAsync("Hello!!");
            }
        }
    }
}
