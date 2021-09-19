using Catalog.Repositories;
using Catalog.settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catalog
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
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));//anytime a Guid is seen, serialize it as a string
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String)); // same w/ datatimes
            var mongoDbsettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>(); // moved from V
            services.AddSingleton<IMongoClient>(ServiceProvider =>
            {//grab instance of settings via mongodbsettings class
                /*var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();*/ //gets us the section
                return new MongoClient(mongoDbsettings.ConnectionString); //the connection string from the MongoDbSettings class namespace 
            });

            //change from using list to store everything to mongo db. 
            services.AddSingleton<IItemsRepository, /*InMenuItemsRepository*/MongoDbItemsRepository>(); // singleton, 1 copy of an instance of a type accross the entire lifetime of our service. will be reused  

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });

            services.AddHealthChecks().AddMongoDb(
                mongoDbsettings.ConnectionString, name: "mongodb", timeout: TimeSpan.FromSeconds(3),
                tags: new[] {"ready"}
                
                );//the arguments make the health check based on wheather or not the api can reach the db
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions { 
                //predicate used to filter what health checks you want 
                    Predicate = (check) => check.Tags.Contains("ready"), //will only include health checks that have the ready tag 
                    ResponseWriter = async(context, report) =>
                    {
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                status = report.Status.ToString(),
                                checks = report.Entries.Select(entry => new
                                {
                                    name = entry.Key,
                                    status = entry.Value.Status.ToString(),
                                    expectation = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                                    duration = entry.Value.Duration.ToString()

                                }
                                )

                            }

                            );

                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });//Route used for health endpoint localhost:xxxx/health goes along w/ line 55. goes along w/ AspNetCore.HealthChecks.MongoDb nuget for DB. need to be inside internal directory to install w/ terminal

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                     
                    Predicate = (_) => false //excludes every health check, will only come back if REST API is alive on server 

                });


            });
        }
    }
}
