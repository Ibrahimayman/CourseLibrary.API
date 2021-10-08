using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Services;
using CourseLibrary.API.Entities;
using CourseLibrary.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;

namespace CourseLibrary.API
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

            services.AddControllers(setup => {
                setup.ReturnHttpNotAcceptable = true;
                setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            }).AddXmlDataContractSerializerFormatters();

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")
            ));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CourseLibrary.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourseLibrary.API v1"));
            }
            else
            {
                // General Message for any exception.

                app.UseExceptionHandler(appBuilder => {

                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An expeption fault happned. try again later");
                    });
                });

            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
