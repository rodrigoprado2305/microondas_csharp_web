var intervalo; // Defina a variável intervalo fora das funções para que possa ser acessada globalmente
var pausado = false; // Variável global para controlar se o aquecimento está pausado

// Adiciona um ouvinte de eventos ao campo programaSelect para atualizar o tempo e a potência
document.getElementById("programaSelect").addEventListener("change", function () {
    var selectedPrograma = this.value;

    // Realiza uma requisição AJAX para obter os detalhes do programa selecionado
    $.ajax({
        type: 'GET',
        url: '/Microondas/GetDetalhesPrograma',
        data: { nomePrograma: selectedPrograma },
        success: function (programa) {
            // Atualiza os campos de tempo e potência com os valores obtidos do backend
            document.getElementById("tempo").value = programa.tempo;
            document.getElementById("potencia").value = programa.potencia;
        },
        error: function () {
            // Trata qualquer erro de requisição
            console.error('Erro ao obter detalhes do programa.');
        }
    });

    // Verifica se o programa selecionado é o padrão (vazio)
    if (selectedPrograma === "") {
        // Define o tempo como zero e a potência como 10
        document.getElementById("tempo").value = "";
        document.getElementById("potencia").value = 10;
    }
});



function iniciarAquecimento() {
    // Limpar intervalo existente, se houver
    clearInterval(intervalo);

    var programaSelecionado = document.getElementById("programaSelect").value;
    var tempo = parseInt(document.getElementById('tempo').value); // Converter para número inteiro
    var potencia = parseInt(document.getElementById('potencia').value); // Converter para número inteiro

    if (programaSelecionado !== "") {
        // Se um programa foi selecionado, chama a função para iniciar o aquecimento com o programa selecionado
        iniciarAquecimentoComProgramaSelecionado(programaSelecionado, tempo, potencia);
    } else {
        // Se nenhum programa foi selecionado, chama a função para iniciar o aquecimento manualmente
        iniciarAquecimentoManual(tempo, potencia);
    }
}


function iniciarAquecimentoComProgramaSelecionado(programaSelecionado, tempo, potencia) {
    // Limpar intervalo existente, se houver
    clearInterval(intervalo);

    // Monta a sequência de pontos com base na potência
    var pontosPorSegundo = " .";
    for (var i = 1; i < potencia; i++) {
        pontosPorSegundo += ".";
    }

    // Realiza uma requisição AJAX para a action "IniciarAquecimentoComProgramaSelecionado" na controller "Microondas"
    $.ajax({
        type: 'POST', // Método HTTP
        url: '/Microondas/IniciarAquecimentoComProgramaSelecionado', // URL da action na controller
        data: { nomePrograma: programaSelecionado, tempo: tempo, potencia: potencia }, // Dados a serem enviados para a controller
        success: function (response) {
            // Função a ser executada em caso de sucesso na requisição
            console.log('Aquecimento iniciado com sucesso:', response);
            // Atualiza a div com a mensagem retornada pela controller
            document.getElementById('mensagem').innerText = response.mensagem;

            // Verificar se há um aquecimento em execução
            if (response.aquecimentoEmExecucao && !pausado) {
                // Se houver, acrescentar 30 segundos ao tempo restante
                tempo += 30;
                console.log('Tempo restante acrescido em 30 segundos:', tempo);
            }

            var tempoTotal = response.tempoTotal; // Obtém o tempoTotal retornado na resposta JSON

            var progresso = document.getElementById('progresso');
            if (tempoTotal != undefined) {
                progresso.innerText = "Iniciando o aquecimento ";
            }

            var contadorPontos = 0;
            intervalo = setInterval(function () { // Use intervalo como variável global
                if (!pausado) { // Verifica se o aquecimento está pausado
                    progresso.innerText += pontosPorSegundo;
                    contadorPontos++;
                    if (contadorPontos >= tempoTotal) {
                        clearInterval(intervalo);
                        progresso.innerText += " .Aquecimento concluído";
                        // Chamada AJAX para notificar a Controller sobre a conclusão do aquecimento
                        $.ajax({
                            type: "POST",
                            url: "/Microondas/NotificarConclusaoAquecimento",
                            success: function (response) {
                                console.log("Aquecimento concluído");
                            },
                            error: function (xhr, status, error) {
                                console.error("Erro ao notificar a conclusão do aquecimento:", error);
                            }
                        });
                    }
                }
            }, 1000);

            // Após o tempo especificado, exibir a mensagem de conclusão
            setTimeout(function () {
                clearInterval(intervalo);
                if (contadorPontos < tempoTotal) {
                    progresso.innerText += " .Aquecimento concluído";
                    // Chamada AJAX para notificar a Controller sobre a conclusão do aquecimento
                    $.ajax({
                        type: "POST",
                        url: "/Microondas/NotificarConclusaoAquecimento",
                        success: function (response) {
                            console.log("Aquecimento concluído");
                        },
                        error: function (xhr, status, error) {
                            console.error("Erro ao notificar a conclusão do aquecimento:", error);
                        }
                    });
                }
            }, tempo * 1000);
        },
        error: function (error) {
            // Função a ser executada em caso de erro na requisição
            console.error('Erro ao iniciar o aquecimento:', error);
            // Aqui você pode lidar com o erro de acordo com a sua aplicação
        }
    });
}

// Função para iniciar o aquecimento manualmente
function iniciarAquecimentoManual(tempo, potencia) {
    // Limpar intervalo existente, se houver
    clearInterval(intervalo);

    // Monta a sequência de pontos com base na potência
    var pontosPorSegundo = " .";
    for (var i = 1; i < potencia; i++) {
        pontosPorSegundo += ".";
    }

    // Realiza uma requisição AJAX para a action "IniciarAquecimento" na controller "Microondas"
    $.ajax({
        type: 'POST', // Método HTTP
        url: '/Microondas/IniciarAquecimento', // URL da action na controller
        data: { tempo: tempo, potencia: potencia }, // Dados a serem enviados para a controller
        success: function (response) {
            // Função a ser executada em caso de sucesso na requisição
            console.log('Aquecimento iniciado com sucesso:', response);
            // Atualiza a div com a mensagem retornada pela controller
            document.getElementById('mensagem').innerText = response.mensagem;

            // Verificar se há um aquecimento em execução
            if (response.aquecimentoEmExecucao && !pausado) {
                // Se houver, acrescentar 30 segundos ao tempo restante
                tempo += 30;
                console.log('Tempo restante acrescido em 30 segundos:', tempo);
            }

            var tempoTotal = response.tempoTotal; // Obtém o tempoTotal retornado na resposta JSON

            var progresso = document.getElementById('progresso');
            if (tempoTotal != undefined) {
                progresso.innerText = "Iniciando o aquecimento ";
            }

            var contadorPontos = 0;
            intervalo = setInterval(function () { // Use intervalo como variável global
                if (!pausado) { // Verifica se o aquecimento está pausado
                    progresso.innerText += pontosPorSegundo;
                    contadorPontos++;
                    if (contadorPontos >= tempoTotal) {
                        clearInterval(intervalo);
                        progresso.innerText += " .Aquecimento concluído";
                        // Chamada AJAX para notificar a Controller sobre a conclusão do aquecimento
                        $.ajax({
                            type: "POST",
                            url: "/Microondas/NotificarConclusaoAquecimento",
                            success: function (response) {
                                console.log("Aquecimento concluído");
                            },
                            error: function (xhr, status, error) {
                                console.error("Erro ao notificar a conclusão do aquecimento:", error);
                            }
                        });
                    }
                }
            }, 1000);

            // Após o tempo especificado, exibir a mensagem de conclusão
            setTimeout(function () {
                clearInterval(intervalo);
                if (contadorPontos < tempoTotal) {
                    progresso.innerText += " .Aquecimento concluído";
                    // Chamada AJAX para notificar a Controller sobre a conclusão do aquecimento
                    $.ajax({
                        type: "POST",
                        url: "/Microondas/NotificarConclusaoAquecimento",
                        success: function (response) {
                            console.log("Aquecimento concluído");
                        },
                        error: function (xhr, status, error) {
                            console.error("Erro ao notificar a conclusão do aquecimento:", error);
                        }
                    });
                }
            }, tempo * 1000);
        },
        error: function (error) {
            // Função a ser executada em caso de erro na requisição
            console.error('Erro ao iniciar o aquecimento:', error);
            // Aqui você pode lidar com o erro de acordo com a sua aplicação
        }
    });
}


function iniciarAquecimentoRapido() {
    var tempo = document.getElementById('tempo').value;
    var potencia = document.getElementById('potencia').value;

    // Verifica se nenhum tempo ou potência foi fornecido
    if (tempo === "" || potencia === "") {
        // Realiza uma requisição AJAX para a action "IniciarAquecimentoRapido" na controller "Microondas"
        $.ajax({
            type: 'POST', // Método HTTP
            url: '/Microondas/IniciarAquecimentoRapido', // URL da action na controller
            success: function (response) {
                // Função a ser executada em caso de sucesso na requisição
                console.log('Aquecimento rápido iniciado com sucesso:', response);
                // Atualiza a div com a mensagem retornada pela controller
                document.getElementById('mensagem').innerText = response.mensagem;
                // Aqui você pode atualizar a interface do usuário conforme necessário
                document.getElementById('tempo').value = 30;
                // Aqui você pode atualizar a interface do usuário conforme necessário

                // Calcular a sequência de pontos com base na potência
                var progresso = document.getElementById('progresso');
                progresso.innerText = "Iniciando o aquecimento ";

                var pontosPorSegundo = ".";
                for (var i = 1; i < potencia; i++) {
                    pontosPorSegundo += ".";
                }

                var contadorPontos = 0;
                intervalo = setInterval(function () {
                    progresso.innerText += pontosPorSegundo;
                    contadorPontos++;
                    if (contadorPontos >= 30) {
                        clearInterval(intervalo);
                        progresso.innerText += " .Aquecimento concluído";
                    }
                }, 1000);
            },
            error: function (error) {
                // Função a ser executada em caso de erro na requisição
                console.error('Erro ao iniciar o aquecimento rápido:', error);
                // Aqui você pode lidar com o erro de acordo com a sua aplicação
            }
        });
    } else {
        // Caso o usuário tenha fornecido o tempo ou a potência, exiba uma mensagem ou realize outra ação conforme necessário
        console.log("O início rápido só pode ser acionado se nenhum tempo ou potência for informado.");
    }

    // Verificar se o botão deve ser desabilitado
    if (tempo !== "" && potencia !== "") {
        document.getElementById('btnInicioRapido').disabled = true;
    } else {
        document.getElementById('btnInicioRapido').disabled = false;
    }
}

function verificarCampos() {
    var tempo = document.getElementById('tempo').value;
    var potencia = document.getElementById('potencia').value;

    // Verificar se o botão deve ser desabilitado
    if (tempo === "" || potencia === "") {
        document.getElementById('btnInicioRapido').disabled = false;
    } else {
        document.getElementById('btnInicioRapido').disabled = true;
    }
}

// Chamar a função de verificação sempre que houver uma mudança nos campos de tempo ou potência
document.getElementById('tempo').addEventListener('input', verificarCampos);
document.getElementById('potencia').addEventListener('input', verificarCampos);

function pausarOuCancelarAquecimento() {
    $.ajax({
        type: 'POST',
        url: '/Microondas/PausarOuCancelarAquecimento',
        success: function (response) {
            console.log('Ação de pausar/cancelar o aquecimento:', response);
            document.getElementById('mensagem').innerText = response.mensagem;

            // Verifica se o aquecimento estava em andamento
            if (!response.emAndamento) {
                // Se o aquecimento não estava em andamento, limpa os campos da view
                document.getElementById('tempo').value = '';
                document.getElementById('potencia').value = 10; // Define a potência como 10
            }

            // Se a ação for pausar, atualiza o botão de iniciar para "Retomar"
            if (response.pausado) {
                document.getElementById('iniciar').innerText = "Retomar";
                // Pausa o contador se estiver pausado
                pausado = true;
                clearInterval(intervalo);
            } else {
                // Se a ação for cancelar, limpa o botão de iniciar
                document.getElementById('iniciar').innerText = "Iniciar Aquecimento";
                var progresso = document.getElementById('progresso');
                progresso.innerText = "";
                // Limpa o campo programaSelect e define-o para exibir a opção padrão
                var programaSelect = document.getElementById("programaSelect");
                programaSelect.value = ""; // Define o valor como vazio para exibir a opção padrão
                // Também pode ser usado programaSelect.selectedIndex = 0; para selecionar a primeira opção

                // Retoma o contador se o aquecimento não estiver mais pausado
                pausado = false;

                // Aguarda 1 segundo antes de recarregar a página
                setTimeout(function () {
                    location.reload();
                }, 999);
            }
        },
        error: function (error) {
            console.error('Erro ao pausar/cancelar o aquecimento:', error);
            // Lidar com o erro de acordo com a sua aplicação
        }
    });
}