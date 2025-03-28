using BennerMicroOndas.Controllers;
using BennerMicroOndas.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BennerMicroOndas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Este método é chamado em tempo de execução. Use este método para adicionar serviços ao contêiner.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IProgramaAquecimentoService, ProgramaAquecimentoService>();
            services.AddControllersWithViews();
            services.AddScoped<MicroondasController>(); // Adiciona MicroondasController como um serviço
            services.AddHttpContextAccessor(); // Adiciona o serviço IHttpContextAccessor
            services.AddSession(); // Adiciona suporte a sessões
        }

        // Este método é chamado em tempo de execução. Use este método para configurar o pipeline de solicitação HTTP.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Adiciona o uso de sessão

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "CadastroPrograma",
                    pattern: "CadastroPrograma",
                    defaults: new { controller = "CadastrarProgamasController", action = "CadastroPrograma" }
                );

                endpoints.MapControllerRoute(
                    name: "microondas",
                    pattern: "microondas/iniciaraquecimento",
                    defaults: new { controller = "Microondas", action = "IniciarAquecimento" }
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
