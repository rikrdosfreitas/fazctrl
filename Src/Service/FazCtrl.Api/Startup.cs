using System;
using System.Diagnostics;
using AutoMapper;
using FazCtrl.Api.Logging;
using FazCtrl.Application.CommandHandler;
using FazCtrl.Common.Metadata;
using FazCtrl.Common.Serializer;
using FazCtrl.Infrastructure;
using FazCtrl.Infrastructure.Azure;
using FazCtrl.Infrastructure.Azure.EventSourcing;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FazCtrl.Api
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

            services
                .AddMediatR(typeof(Program), typeof(GrazingCommandHandler))
                .AddAutoMapper(typeof(Program));

            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));


            services.AddSingleton<ITextSerializer, StandardSerializer>();
            services.AddSingleton<IMetadataProvider, StandardMetadataProvider>();

            services.AddSingleton<IRepository, EventStore>();
            //services.AddTransient(typeof(IEventSourcedRepository<>), typeof(AzureEventSourceRepository<>));
            services.AddTransient(typeof(IEventStoreRepository<>), typeof(EvenSourceRepository<>));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Action<RequestResponseLoggingMiddleware.RequestProfilerModel> requestResponseHandler = requestProfilerModel =>
            {
                Debug.Print(requestProfilerModel.Request);
                Debug.Print(Environment.NewLine);
                Debug.Print(requestProfilerModel.Response);
            };
            app.UseMiddleware<RequestResponseLoggingMiddleware>(requestResponseHandler);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
