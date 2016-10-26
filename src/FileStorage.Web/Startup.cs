using System;
using System.IO;
using FileStorage.DAL;
using FileStorage.DAL.Configuration;
using FileStorage.DAL.Contracts;
using FileStorage.DAL.Contracts.Initializers;
using FileStorage.DAL.Contracts.Repositories;
using FileStorage.DAL.Repositories;
using FileStorage.Domain.Entities;
using FileStorage.Services.Contracts;
using FileStorage.Services.Services;
using FileStorage.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger.Model;

namespace FileStorage.Web
{
    public partial class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var connectionStringConfig = builder.Build();

            services.AddDbContext<DataDbContext>(opt => opt.UseSqlServer(
                connectionStringConfig.GetConnectionString("DefaultConnection")));

            //services.AddDbContext<DataDbContext>(opt => opt.UseInMemoryDatabase());

            services.AddIdentity<ApplicationUser, IdentityRole>(pass =>
                 {
                     pass.Password.RequireDigit = false;
                     pass.Password.RequiredLength = 6;
                     pass.Password.RequireNonAlphanumeric = false;
                     pass.Password.RequireUppercase = false;
                     pass.Password.RequireLowercase = false;
                 }).AddEntityFrameworkStores<DataDbContext>();


            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "File storage",
                    Description = "API documentation",
                    TermsOfService = "None"
                });
                options.IncludeXmlComments(GetXmlCommentsPath(PlatformServices.Default.Application));
                options.OperationFilter<FileOperation>();
                options.DescribeAllEnumsAsStrings();
            });

            // DI
            // Max lenght of file is 4 GB
            services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 4294967295);
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<IBlobService, AzureBlobService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INodeRepository, NodeRepository>();
            services.AddScoped<IFileVersionRepository, FileVersionRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IDatabaseInitializer databaseInitializer, IServiceProvider services)
        {
            ConfigureAuth(app, services);

            app.UseStaticFiles();
            app.UseCors(builder =>
                    // This will allow any request from any server. 
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());


            AutomapperConfiguration.Load();

            // Add MVC to the request pipeline.
            app.UseDeveloperExceptionPage();
            app.UseMvc();

            app.UseSwagger((httpRequest, swaggerDoc) =>
            {
                swaggerDoc.Host = httpRequest.Host.Value;
            });

            app.UseSwaggerUi();
            app.UseMvcWithDefaultRoute();


            // Recreate db's
            databaseInitializer.Seed().GetAwaiter().GetResult();
        }
        private string GetXmlCommentsPath(ApplicationEnvironment appEnvironment)
        {
            return Path.Combine(appEnvironment.ApplicationBasePath, "FileStorage.Web.xml");
        }
    }

}
