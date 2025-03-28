using System.ComponentModel.DataAnnotations;

namespace BennerMicroOndas.Models
{
    public class ProgramaAquecimento
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Alimento é obrigatório.")]
        public string Alimento { get; set; }

        [Required(ErrorMessage = "O campo Tempo é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O campo Tempo deve ser maior que zero.")]
        public int? Tempo { get; set; }

        [Required(ErrorMessage = "O campo Potência é obrigatório.")]
        [Range(1, 10, ErrorMessage = "O campo Potência deve estar entre 1 e 10.")]
        public int? Potencia { get; set; }

        [Required(ErrorMessage = "O campo Instruções é obrigatório.")]
        public string Instrucoes { get; set; }

        public string ErrorMessage { get; set; }
        public bool PreDefinido { get; set; }
        public bool Customizado { get; set; }
    }
}
