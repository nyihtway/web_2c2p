using _2C2P.DEMO.Infrastructure.AutoMapper;
using _2C2P.DEMO.WebApp.AutofacModules;
using _2C2P.DEMO.WebApp.Services.Jaeger;
using _2C2P.DEMO.WebApp.Services.Kafka;
using Autofac;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.IO;

namespace _2C2P.DEMO.WebApp
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddHttpClient()
                .AddHttpContextAccessor();

            services.AddKafka();
            services.AddJaeger();

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddScoped<IKafkaService, KafkaService>();

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AddProfile<MappingProfile>();
            });

            services.AddSingleton(mappingConfig.CreateMapper());

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Version = "v1",
                    Title = "TRANSACTION API",
                    Description = "2C2P Transaction Demo"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(Configuration.GetSection("MongoDBConnection:ConnectionString").Value));

            builder.RegisterModule(new InfrastructureModule(new MongoClient(clientSettings),
                       Configuration.GetSection("MongoDBConnection:Database").Value));

            builder.RegisterModule(new BsonModule());
            //builder.RegisterModule(new MediatorModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();
            app.UseJaeger();

            app.UseSwaggerUI(
            options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "TRANSACTION API");

            });

            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"UploadFiles")),
                RequestPath = new PathString("/UploadFiles")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
