using BennerMicroOndas.Extensions;
using BennerMicroOndas.Models;
using BennerMicroOndas.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

public class MicroondasController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProgramaAquecimentoService _programaAquecimentoService;
    public MicroondasController(IHttpContextAccessor httpContextAccessor, IProgramaAquecimentoService programaAquecimentoService)
    {
        _httpContextAccessor = httpContextAccessor;
        _programaAquecimentoService = programaAquecimentoService;
    }

    //private List<ProgramaAquecimento> programasPreDefinidos = new List<ProgramaAquecimento>
    //{
    //    new ProgramaAquecimento
    //    {
    //        Nome = "Pipoca",
    //        Alimento = "Pipoca (de micro-ondas)",
    //        Tempo = 180, // 3 minutos em segundos
    //        Potencia = 7,
    //        StringAquecimento = "xxxxx", // String de aquecimento específica
    //        Instrucoes = "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento."
    //    },
    //    // Adicione outros programas pré-definidos aqui...
    //};

    [HttpPost]
    public IActionResult NotificarConclusaoAquecimento()
    {
        // Defina AquecimentoEmExecucao como false
        _httpContextAccessor.HttpContext.Session.SetString("AquecimentoEmExecucao", false.ToString());
        return Ok();
    }
    private bool AquecimentoEmExecucao
    {
        get
        {
            var value = _httpContextAccessor.HttpContext.Session.GetString("AquecimentoEmExecucao");
            return !string.IsNullOrEmpty(value) && bool.Parse(value);
        }
        set => _httpContextAccessor.HttpContext.Session.SetString("AquecimentoEmExecucao", value.ToString());
    }
    private bool AquecimentoPausado
    {
        get
        {
            var value = _httpContextAccessor.HttpContext.Session.GetString("AquecimentoPausado");
            return !string.IsNullOrEmpty(value) && bool.Parse(value);
        }
        set => _httpContextAccessor.HttpContext.Session.SetString("AquecimentoPausado", value.ToString());
    }
    private int TempoOriginal
    {
        get => _httpContextAccessor.HttpContext.Session.GetInt32("TempoOriginal").GetValueOrDefault(0);
        set => _httpContextAccessor.HttpContext.Session.SetInt32("TempoOriginal", value);
    }
    private int ContadorClicadas
    {
        get => _httpContextAccessor.HttpContext.Session.GetInt32("ContadorClicadas").GetValueOrDefault(0);
        set => _httpContextAccessor.HttpContext.Session.SetInt32("ContadorClicadas", value);
    }
    private DateTime? TempoInicio
    {
        get => _httpContextAccessor.HttpContext.Session.Get<DateTime?>("TempoInicio");
        set => _httpContextAccessor.HttpContext.Session.Set<DateTime?>("TempoInicio", value);
    }
    [HttpPost]
    public IActionResult IniciarAquecimento(int tempo, int potencia = 10)
    {
        if (!AquecimentoEmExecucao)
        {
            // Se não houver aquecimento em execução, marca como em execução e define o tempo original, contador e o tempo de início
            AquecimentoEmExecucao = true;
            AquecimentoPausado = false; // Define como não pausado ao iniciar o aquecimento
            TempoOriginal = tempo;
            ContadorClicadas = 0;
            TempoInicio = DateTime.Now;
        }
        else
        {
            // Se houver um aquecimento em execução e estiver pausado, retoma o aquecimento
            if (AquecimentoPausado)
            {
                
                TempoInicio = DateTime.Now; // Atualiza o tempo de início
            }
            else
            {
                // Se houver um aquecimento em execução, incrementa o contador de cliques
                ContadorClicadas++;
                TempoOriginal = tempo;
            }
        }

        // Calcula o tempo decorrido desde o início do aquecimento até o momento atual
        TimeSpan tempoDecorrido = DateTime.Now - TempoInicio.GetValueOrDefault(DateTime.Now);
        int tempoTotal = TempoOriginal;


        if (!AquecimentoPausado)
        {
            // Calcula o tempo total considerando o tempo original e o incremento progressivo
            tempoTotal = TempoOriginal + (30 * ContadorClicadas);
            AquecimentoPausado = false;
        }


        // Calcula o tempo restante subtraindo o tempo decorrido do tempo total
        int tempoRestante = tempoTotal - (int)tempoDecorrido.TotalSeconds;

        if (tempoTotal > 120)
        {
            tempoTotal = 119;
            // Retorna a mensagem de limite de tempo
            return Json(new
            {
                mensagem = "Não pode incrementar mais 30 segundos pois só permite até 120 segundos de tempo. " +
                                         $"\nIniciando aquecimento com tempo: {(tempoTotal / 60):D2}:{(tempoTotal % 60):D2}, potência: {potencia}.",
                tempoTotal = tempoTotal // Inclui o tempoTotal na resposta JSON 
            });
        }

        // Validação do tempo mínimo e máximo
        if (tempoRestante < 1)
        {
            AquecimentoEmExecucao = false;
            return Json(new { mensagem = "Tempo inválido. O tempo deve estar entre 1 e 120 segundos." });
        }

        // Validação da potência
        if (potencia < 1 || potencia > 10)
        {
            AquecimentoEmExecucao = false;
            return Json(new { mensagem = "Potência inválida. A potência deve estar entre 1 e 10." });
        }

        // Conversão do tempo em minutos, se necessário
        if (tempoRestante > 60)
        {
            int minutos = tempoRestante / 60;
            int segundos = tempoRestante % 60;
            return Json(new
            {
                mensagem = $"Iniciando aquecimento com tempo: {minutos}:{segundos:D2}, potência: {potencia}.",
                aquecimentoEmExecucao = AquecimentoEmExecucao, // Status do aquecimento
                tempoTotal = tempoTotal, // Inclui o tempoTotal na resposta JSON 
                emAndamento = true, pausado = false
            });
        }

        return Json(new
        {
            mensagem = $"Iniciando aquecimento com tempo: {tempoRestante} segundos, potência: {potencia}.",
            aquecimentoEmExecucao = AquecimentoEmExecucao, // Status do aquecimento
            tempoTotal = tempoTotal, // Inclui o tempoTotal na resposta JSON 
            emAndamento = true, pausado = false
        });
    }

    [HttpPost]
    public IActionResult IniciarAquecimentoRapido(int? tempo, int? potencia)
    {
        // Verifica se nenhum tempo ou potência foi fornecido
        if (tempo.HasValue || potencia.HasValue)
        {
            return Json(new { mensagem = "O início rápido só pode ser acionado se nenhum tempo ou potência for informado." });
        }

        // Definir AquecimentoEmExecucao como true
        AquecimentoEmExecucao = true;

        int tempoPadrao = 30; // Tempo padrão para o início rápido
        int potenciaPadrao = 10; // Potência padrão para o início rápido

        return Json(new { mensagem = $"Iniciando aquecimento rápido com tempo: {tempoPadrao} segundos, potência: {potenciaPadrao}." });
    }

    [HttpPost]
    public ActionResult PausarOuCancelarAquecimento()
    {
        // Verifica se o aquecimento está em andamento
        if (AquecimentoEmExecucao)
        {
            // Verifica se o aquecimento está pausado
            if (AquecimentoPausado)
            {
                // Se estiver pausado, cancela o aquecimento e limpa as informações
                AquecimentoEmExecucao = false;
                AquecimentoPausado = false;
                LimparInformacoes();
                return Json(new { mensagem = "Aquecimento cancelado.", emAndamento = false, pausado = false });
            }
            else
            {
                // Se o aquecimento estiver em andamento, mas não estiver pausado, pausa o aquecimento
                AquecimentoPausado = true;
                return Json(new { mensagem = "Aquecimento pausado.", emAndamento = true, pausado = true });
            }
        }
        else
        {
            // Se o aquecimento não estiver em andamento, limpa as informações
            LimparInformacoes();
            return Json(new { mensagem = "Informações de tempo e potência foram redefinidas.", emAndamento = false, pausado = false });
        }
    }

    private void LimparInformacoes()
    {
        // Encerra todos os aquecimentos ativos
        AquecimentoEmExecucao = false;

        // Limpa todos os campos, exceto o campo de potência
        TempoOriginal = 0;
        ContadorClicadas = 0;
        TempoInicio = null;
    }

    [HttpPost]
    public IActionResult IniciarAquecimentoComProgramaSelecionado(string nomePrograma, int tempo, int potencia)
    {
        var programasPreDefinidos = _programaAquecimentoService.GetProgramasPreDefinidos();
        var programa = programasPreDefinidos.FirstOrDefault(p => p.Nome == nomePrograma);
        if (programa != null)
        {

            if (!AquecimentoEmExecucao)
            {
                // Se não houver aquecimento em execução, marca como em execução e define o tempo original, contador e o tempo de início
                AquecimentoEmExecucao = true;
                AquecimentoPausado = false; // Define como não pausado ao iniciar o aquecimento
                TempoOriginal = tempo;
                ContadorClicadas = 0;
                TempoInicio = DateTime.Now;
            }
            else
            {
                // Se houver um aquecimento em execução e estiver pausado, retoma o aquecimento
                if (AquecimentoPausado)
                {

                    TempoInicio = DateTime.Now; // Atualiza o tempo de início
                }
                else
                {
                    // Se houver um aquecimento em execução, incrementa o contador de cliques
                    ContadorClicadas++;
                    TempoOriginal = tempo;
                }
            }

            // Calcula o tempo decorrido desde o início do aquecimento até o momento atual
            TimeSpan tempoDecorrido = DateTime.Now - TempoInicio.GetValueOrDefault(DateTime.Now);
            int tempoTotal = TempoOriginal;


            if (!AquecimentoPausado)
            {
                // Calcula o tempo total considerando o tempo original e o incremento progressivo
                tempoTotal = TempoOriginal + (30 * ContadorClicadas);
                AquecimentoPausado = false;
            }


            // Calcula o tempo restante subtraindo o tempo decorrido do tempo total
            int tempoRestante = tempoTotal - (int)tempoDecorrido.TotalSeconds;

            // Conversão do tempo em minutos, se necessário
            if (tempoRestante > 60)
            {
                int minutos = tempoRestante / 60;
                int segundos = tempoRestante % 60;
                return Json(new
                {
                    mensagem = $"Iniciando aquecimento com tempo: {minutos}:{segundos:D2}, potência: {potencia}.",
                    aquecimentoEmExecucao = AquecimentoEmExecucao, // Status do aquecimento
                    tempoTotal = tempoTotal, // Inclui o tempoTotal na resposta JSON 
                    emAndamento = true,
                    pausado = false
                });
            }

            return Json(new
            {
                mensagem = $"Iniciando aquecimento com tempo: {tempoRestante} segundos, potência: {potencia}.",
                aquecimentoEmExecucao = AquecimentoEmExecucao, // Status do aquecimento
                tempoTotal = tempoTotal, // Inclui o tempoTotal na resposta JSON 
                emAndamento = true,
                pausado = false
            });
        }
        else
        {
            return Json(new { mensagem = "Tempo invalido." });
        }
    }

    //public List<ProgramaAquecimento> GetProgramasPreDefinidos()
    //{
    //    return programasPreDefinidos;
    //}

    // Action para obter os detalhes do programa pelo nome
    [HttpGet]
    public IActionResult GetDetalhesPrograma(string nomePrograma)
    {
        var programasPreDefinidos = _programaAquecimentoService.GetProgramasPreDefinidos();
        var programa = programasPreDefinidos.FirstOrDefault(p => p.Nome == nomePrograma);
        if (programa != null)
        {
            return Json(programa); // Retorna os detalhes do programa em formato JSON
        }
        else
        {
            return NotFound(); // Retorna um erro 404 se o programa não for encontrado
        }
    }
}
