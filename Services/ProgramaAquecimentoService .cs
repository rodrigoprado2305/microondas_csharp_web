using BennerMicroOndas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BennerMicroOndas.Services
{
    public interface IProgramaAquecimentoService
    {
        List<ProgramaAquecimento> GetProgramasPreDefinidos();
        void SalvarPrograma(ProgramaAquecimento programa);
    }

    public class ProgramaAquecimentoService : IProgramaAquecimentoService
    {
        private readonly string _filePath = "programas.json";
        private List<ProgramaAquecimento> _programasPreDefinidos;

        public ProgramaAquecimentoService()
        {
            try
            {
                // Carrega os programas predefinidos do arquivo JSON, se existir
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _programasPreDefinidos = JsonSerializer.Deserialize<List<ProgramaAquecimento>>(json);
                }
                else
                {
                    // Inicialize a lista de programas predefinidos
                    _programasPreDefinidos = new List<ProgramaAquecimento>
                    {
                        new ProgramaAquecimento
                        {
                            Nome = "Pipoca",
                            Alimento = "Pipoca (de micro-ondas)",
                            Tempo = 180, // 3 minutos em segundos
                            Potencia = 7,
                            Instrucoes = "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento."
                        },
                        new ProgramaAquecimento
                        {
                            Nome = "Leite",
                            Alimento = "Leite",
                            Tempo = 300, // 5 minutos em segundos
                            Potencia = 5,
                            Instrucoes = "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras."
                        },
                        new ProgramaAquecimento
                        {
                            Nome = "Carnes de boi",
                            Alimento = "Carne em pedaço ou fatias",
                            Tempo = 840, // 14 minutos em segundos
                            Potencia = 4,
                            Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."
                        },
                        new ProgramaAquecimento
                        {
                            Nome = "Frango",
                            Alimento = "Frango (qualquer corte)",
                            Tempo = 480, // 8 minutos em segundos
                            Potencia = 7,
                            Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."
                        },
                        new ProgramaAquecimento
                        {
                            Nome = "Feijão",
                            Alimento = "Feijão congelado",
                            Tempo = 480, // 8 minutos em segundos
                            Potencia = 9,
                            Instrucoes = "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas."
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                // Trate a exceção de forma apropriada, como por exemplo, registrar o erro em um log
                Console.WriteLine($"Ocorreu um erro ao carregar os programas predefinidos: {ex.Message}");
            }
        }

        public List<ProgramaAquecimento> GetProgramasPreDefinidos()
        {
            return _programasPreDefinidos;
        }

        public void SalvarPrograma(ProgramaAquecimento programa)
        {
            // Valida se o nome do programa já existe
            if (_programasPreDefinidos.Any(p => p.Nome == programa.Nome))
            {
                // Exibe uma mensagem de alerta
                programa.ErrorMessage = "Já existe um programa com esse nome.";
                return; // Sai do método sem fazer nada adicional
            }
            
            programa.Customizado = true;

            try
            {
                // Adiciona o novo programa à lista
                _programasPreDefinidos.Add(programa);

                // Serializa a lista de programas em formato JSON
                string json = JsonSerializer.Serialize(_programasPreDefinidos);

                // Escreve o JSON em um arquivo
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                // Trate a exceção de forma apropriada, como por exemplo, registrar o erro em um log
                Console.WriteLine($"Ocorreu um erro ao salvar o programa: {ex.Message}");
            }
        }
    }
}