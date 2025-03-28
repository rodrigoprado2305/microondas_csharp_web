using BennerMicroOndas.Models;
using BennerMicroOndas.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using BennerMicroOndas.Services;

namespace BennerMicroOndas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MicroondasController _microondasController;
        private readonly IProgramaAquecimentoService _programaAquecimentoService;

        public HomeController(ILogger<HomeController> logger, MicroondasController microondasController, IProgramaAquecimentoService programaAquecimentoService)
        {
            _logger = logger;
            _microondasController = microondasController;
            _programaAquecimentoService = programaAquecimentoService;
        }

        public IActionResult Index()
        {
            //var programasPreDefinidos = _microondasController.GetProgramasPreDefinidos();
            var programasPreDefinidos = _programaAquecimentoService.GetProgramasPreDefinidos();
            return View(programasPreDefinidos);
            //return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
