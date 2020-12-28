using Cryptography.Wrappers;
using Cryptography.Wrappers.Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SecretSantaTelegramBot.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ngrok.AspNetCore;
using System.IO;
using System.Reflection;

namespace SecretSantaTelegramBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Enviroment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Enviroment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.AddSingleton<IEncryptor, EncryptorService>();
            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));
            services.Configure<ThumbprintCertificateInfo>(Configuration.GetSection("ThumbprintCertificateInfo"));
            services.AddSingleton<NotificationService>();
            services.AddSingleton<DrawService>();

            services.AddNgrok();

            if (Enviroment.IsProduction())
            {
                services.AddNgrok(options =>
                {
                    options.Disable = false;
                    options.DetectUrl = true;
                    options.ManageNgrokProcess = true;
                    options.DownloadNgrok = true;
                    options.ProcessStartTimeoutMs = 5000;
                    options.RedirectLogs = true;
                    options.NgrokConfigProfile = null;
                    options.ApplicationHttpUrl = null;
                    options.NgrokPath = null;
                });
            }

            services.AddDbContext<SecretSantaContext>(
                    options => options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"),
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(10),
                                errorNumbersToAdd: null);
                        }),
                    ServiceLifetime.Transient,
                    ServiceLifetime.Transient);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecretSantaTelegramBot", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", "SecretSantaTelegramBot v1"));

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
