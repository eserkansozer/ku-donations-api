﻿using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using offset_my_carbon_dal.Data;
using offset_my_carbon_dal.Repositories;

namespace offset_my_carbon_api
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
            services.AddDbContext<DataContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"),
            builder => builder.MigrationsAssembly("offset-my-carbon-api"))
            );

            services.AddTransient<Seed>();
            services.AddTransient<IDonationsRepository, DonationsSqlRepository>();
            //services.AddTransient<IEmailService, SmtpEmailService>();

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.Headers.Add("Application-Error", error.Error.Message);
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        }

                        return null;
                    });
                });
            }

            seeder.SeedDonations();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
