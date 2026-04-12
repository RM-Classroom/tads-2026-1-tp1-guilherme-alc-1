using System.ComponentModel.DataAnnotations;
using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record VeiculoRequestDTO(
        [Required(ErrorMessage = "O modelo é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O modelo deve ter entre 2 e 100 caracteres.")]
        string Modelo,

        [Range(1900, 2100, ErrorMessage = "Ano inválido")]
        int Ano,

        [Range(0, int.MaxValue, ErrorMessage = "A quilometragem deve ser maior ou igual a zero")]
        int Quilometragem,

        [Required(ErrorMessage = "A placa é obrigatória")]
        [RegularExpression(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$", ErrorMessage = "Placa inválida")]
        string Placa,

        [StringLength(40, ErrorMessage = "A cor deve conter no máximo 40 caracteres.")]
        string? Cor,

        TipoCombustivel Combustivel,

        bool Disponivel,

        [Range(1, long.MaxValue, ErrorMessage = "FabricanteId deve ser válido")]
        long FabricanteId
    );
}
