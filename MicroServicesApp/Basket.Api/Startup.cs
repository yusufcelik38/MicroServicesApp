using Basket.Api.Data;
using Basket.Api.Data.Interfaces;
using Basket.Api.Repository;
using Basket.Api.Repository.Interfaces;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventBusRabbitMQ.Producer;

namespace Basket.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(Configuration
                    .GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddTransient<IBasketContext, BasketContext>();
            services.AddTransient<IBasketRepository, BasketRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket Microservice", Version = "v1" });
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<IRabbitMQConnection>(
                sp =>
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = Configuration["EventBus:HostName"],
                        UserName = Configuration["EventBus:UserName"],
                        Password = Configuration["EventBus:Password"]
                    };
                    return new RabbitMQConnection(factory);
                });
            services.AddSingleton<EventBusRabbitMQProducer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket Microservice V1");
            });
        }
    }
}
