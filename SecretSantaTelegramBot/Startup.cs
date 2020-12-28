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

namespace SecretSantaTelegramBot
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
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.AddSingleton<IEncryptor, EncryptorService>();
            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));
            services.Configure<ThumbprintCertificateInfo>(Configuration.GetSection("ThumbprintCertificateInfo"));
            services.AddSingleton<NotificationService>();
            services.AddSingleton<DrawService>();
            services.AddNgrok();

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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecretSantaTelegramBot v1"));

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
