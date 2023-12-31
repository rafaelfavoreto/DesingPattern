using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using src.Data;
using src.Data.Repositories;
using src.Domain;

namespace EFCore.UowRepository
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

            services.AddControllers()
            .AddNewtonsoftJson(options =>{
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EFCore.UowRepository", Version = "v1" });
            });

            services.AddDbContext<ApplicationContext>(p=>p.UseSqlServer( "Server=Local\\DBFavoreto; Database=UoW;Integrated Security=True;"));
            services.AddScoped<IDepartamentoRepository,DepartamentoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) //Construtor de aplicação , Construtor de Ambiente
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EFCore.UowRepository v1"));
            }

            InicializarBaseDeDados(app);
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });         
        }

        private void InicializarBaseDeDados(IApplicationBuilder app) // quando chamar o aplicationbuilder ele chama aqui tb 
        {
            using var db = app
                .ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ApplicationContext>();

            if(db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(Enumerable.Range(1,10)
                    .Select(p=> new Departamento
                    {
                        Descricao = $"Departamento - {p}",
                        Colaboradores = Enumerable.Range(1,10)
                            .Select(x=> new Colaborador
                            {
                                Nome = $"Colaborador: {x}/{p}"
                            }).ToList()
                    } ));

                db.SaveChanges();    
            }    
        }
    }
}
