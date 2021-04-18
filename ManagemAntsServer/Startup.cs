using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer
{
    public class Startup
    {
        private string _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer()
                        .AddDbContext<DataAccess.EfModels.ManagemAntsDbContext>(options => options.UseSqlServer(_connectionString));
            services.AddAutoMapper(typeof(DataAccess.AutomapperProfiles));
            services.AddRazorPages();
            services.AddControllers();
            services.AddTransient<DataAccess.Interfaces.IProjectRepository, DataAccess.Repositories.ProjectRepository>();
            services.AddTransient<DataAccess.Interfaces.IUserRepository, DataAccess.Repositories.UserRepository>();
            services.AddTransient<DataAccess.Interfaces.IProjectsHasUserRepository, DataAccess.Repositories.ProjectsHasUserRepository>();
            services.AddTransient<DataAccess.Interfaces.ITaskRepository, DataAccess.Repositories.TaskRepository>();
            services.AddTransient<DataAccess.Interfaces.IUsersHasTaskRepository, DataAccess.Repositories.UsersHasTaskRepository>();

            services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ManagemAntsServer", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _connectionString = Configuration.GetConnectionString("ManagemAnts");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ManagemAntsServer v1"));
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
