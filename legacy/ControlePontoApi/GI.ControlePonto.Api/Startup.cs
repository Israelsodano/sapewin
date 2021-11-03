using GI.ControlePonto.Business;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Interfaces;
using GI.ControlePonto.Repository;
using GI.ControlePonto.Repository.Contexts;
using GI.ControlePonto.Repository.Units;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GI.ControlePonto.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //ConfigurationContext
            services.AddScoped<DbContextOptionsBuilder<SapeWinContext>>(x=> 
                new DbContextOptionsBuilder<SapeWinContext>()
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<DbContextOptionsBuilder<DpaRepContext>>(x=> 
                new DbContextOptionsBuilder<DpaRepContext>()
                    .UseSqlServer(Configuration.GetConnectionString("DefaultOriginConnection")));

            //Contexts
            services.AddScoped<SapeWinContext>();
            services.AddScoped<IUnitOfWork<SapeWinContext>, SapewinUnit>();
            services.AddScoped<DpaRepContext>();
            services.AddScoped<IUnitOfWork<DpaRepContext>, DpaRepUnit>();

            //Repositories
            services.AddScoped<IRepository<Departamentos>, Repository<Departamentos, SapeWinContext>>();
            services.AddScoped<IRepository<Setores>, Repository<Setores, SapeWinContext>>();
            services.AddScoped<IRepository<Funcionarios>, Repository<Funcionarios, SapeWinContext>>();
            services.AddScoped<IRepository<Empresas>, Repository<Empresas, SapeWinContext>>();
            services.AddScoped<IRepository<ApontamentoSujo>, Repository<ApontamentoSujo, DpaRepContext>>();
            services.AddScoped<IRepository<Marcacoes>, Repository<Marcacoes, SapeWinContext>>();
            services.AddScoped<IRepository<FilaProcesso>, Repository<FilaProcesso, SapeWinContext>>();
            services.AddScoped<IRepository<Ponto>, Repository<Ponto, SapeWinContext>>();
            services.AddScoped<IRepository<PontoPares>, Repository<PontoPares, SapeWinContext>>();
            services.AddScoped<IRepository<Horarios>, Repository<Horarios, SapeWinContext>>();
            services.AddScoped<IRepository<Afastamentos>, Repository<Afastamentos, SapeWinContext>>();
            services.AddScoped<IRepository<FeriadosGerais>, Repository<FeriadosGerais, SapeWinContext>>();

            //Business
            services.AddScoped<FuncionarioBusiness>();
            services.AddScoped<DepartamentoBusiness>();
            services.AddScoped<SetorBusiness>();
            services.AddScoped<EmpresaBusiness>();
            services.AddScoped<MotordeProcessamento>();
            services.AddScoped<ApontamentoSujoBusiness>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                (builder) => 
                {
                    builder.AllowAnyOrigin(); 
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("AllowAnyOrigin");
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
