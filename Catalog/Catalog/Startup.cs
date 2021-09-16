using Catalog.Repositories;
using Catalog.settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddSingleton<IMongoClient>(ServiceProvider =>
            {//grab instance of settings via mongodbsettings class
                var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>(); //gets us the section
                return new MongoClient(settings.ConnectionString); //the connection string from the MongoDbSettings class namespace 
            });

            //change from using list to store everything to mongo db. 
            services.AddSingleton<IItemsRepository, /*InMenuItemsRepository*/MongoDbItemsRepository>(); // singleton, 1 copy of an instance of a type accross the entire lifetime of our service. will be reused  

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });
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
            });
        }
    }
}
