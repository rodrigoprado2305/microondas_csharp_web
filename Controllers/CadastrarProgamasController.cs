using BennerMicroOndas.Extensions;
using BennerMicroOndas.Models;
using BennerMicroOndas.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

public class CadastrarProgamasController : Controller
{
    private readonly IProgramaAquecimentoService _programaAquecimentoService;

    public CadastrarProgamasController(IProgramaAquecimentoService programaAquecimentoService)
    {
        _programaAquecimentoService = programaAquecimentoService;
    }

    [Route("CadastroPrograma")]
    public IActionResult CadastroPrograma()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SalvarPrograma(ProgramaAquecimento programa)
    {
        if (!ModelState.IsValid)
        {
            // Se houver erros de validação, retorna a view de cadastro com os erros
            return View("CadastroPrograma", programa);
        }

        // Aqui você pode chamar o serviço para salvar o programa
        _programaAquecimentoService.SalvarPrograma(programa);

        string erroMessage = programa.ErrorMessage;
        if (!string.IsNullOrEmpty(erroMessage))
        {
            // Redireciona para alguma ação após o salvamento bem-sucedido
            return View("CadastroPrograma", programa);
        }
        else
        {
            // Redireciona para alguma ação após o salvamento bem-sucedido
            return RedirectToAction("Index", "Home");
        }
        
    }


    //[HttpGet]
    //public IActionResult DetalhesPrograma(int id)
    //{
    //    // Aqui você pode chamar o serviço para obter os detalhes do programa pelo ID
    //    var programa = _programaAquecimentoService.ObterProgramaPorId(id);

    //    if (programa == null)
    //    {
    //        // Se o programa não for encontrado, retorna um erro 404
    //        return NotFound();
    //    }

    //    // Retorna a view com os detalhes do programa
    //    return View(programa);
    //}

    //[HttpGet]
    //public IActionResult EditarPrograma(int id)
    //{
    //    // Aqui você pode chamar o serviço para obter os detalhes do programa pelo ID
    //    var programa = _programaAquecimentoService.ObterProgramaPorId(id);

    //    if (programa == null)
    //    {
    //        // Se o programa não for encontrado, retorna um erro 404
    //        return NotFound();
    //    }

    //    // Retorna a view de edição com os dados do programa
    //    return View(programa);
    //}

    //[HttpPost]
    //public IActionResult EditarPrograma(ProgramaAquecimento programa)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        // Se houver erros de validação, retorna a view de edição com os erros
    //        return View("EditarPrograma", programa);
    //    }

    //    // Aqui você pode chamar o serviço para atualizar o programa
    //    _programaAquecimentoService.AtualizarPrograma(programa);

    //    // Redireciona para alguma ação após a atualização bem-sucedida
    //    return RedirectToAction("DetalhesPrograma", new { id = programa.Id });
    //}

    //[HttpPost]
    //public IActionResult ExcluirPrograma(int id)
    //{
    //    // Aqui você pode chamar o serviço para excluir o programa pelo ID
    //    _programaAquecimentoService.ExcluirPrograma(id);

    //    // Redireciona para alguma ação após a exclusão bem-sucedida
    //    return RedirectToAction("Index", "Home");
    //}
}
