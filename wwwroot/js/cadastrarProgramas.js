$(document).ready(function () {
    $('#form').submit(function (e) {
        e.preventDefault(); // Evita o envio padrão do formulário

        // Serializa os dados do formulário em formato de string
        var formData = $(this).serialize();

        // Envia os dados via AJAX
        $.ajax({
            type: 'POST', // Método HTTP
            url: '/CadastrarProgamas/SalvarPrograma',
            contentType: 'application/x-www-form-urlencoded',
            data: formData, // Dados do formulário serializados
            success: function (response) {
                // Manipula a resposta da controller, se necessário
                console.log('Cadastro realizado com sucesso!');
            },
            error: function (xhr, status, error) {
                // Manipula erros de requisição
                console.error('Erro ao cadastrar programa:', error);
            }
        });
    });

});