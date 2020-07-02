using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            AgregarServiciosRedis(services);
        }

        private static void AgregarServiciosRedis(IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var redisHost = configuration["Redis:Host"];

                if (string.IsNullOrEmpty(redisHost))
                    throw new RedisConnectionException(
                        ConnectionFailureType.UnableToConnect, "No se encontró la configuración de redis");

                return ConnectionMultiplexer.Connect(redisHost);
            });
            services.AddScoped(provider =>
                provider
                    .GetRequiredService<IConnectionMultiplexer>()
                    .GetDatabase()
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}