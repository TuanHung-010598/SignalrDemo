using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;


[assembly: OwinStartup(typeof(SignalrChat.Startup))]

namespace SignalrChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.Configuration.DefaultMessageBufferSize = 10;
            var config = new RedisScaleoutConfiguration("127.0.0.1:6379", "RealTimeApplication")
            {
                Database = 0
            };

            GlobalHost.DependencyResolver.UseStackExchangeRedis(config);

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                
                map.RunSignalR();
            });
        }
    }
}
