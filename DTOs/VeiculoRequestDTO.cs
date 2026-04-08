using System.ComponentModel.DataAnnotations;
using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record VeiculoRequestDTO(
        [Required(ErrorMessage = "O modelo é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O modelo deve ter entre 2 até 100 caracteres.")]
        string Modelo,
        [Required(ErrorMessage = "O ano é obrigatório")]
        int Ano,
        [Required(ErrorMessage = "A quilometragem é obrigatória")]
        int Quilometragem,
        [Required(ErrorMessage = "A placa é obrigatória")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "A placa deve conter 7 caracteres.")]
        string Placa,
        [StringLength(40, ErrorMessage = "A cor deve conter no máximo 40 caracteres.")]
        string? Cor,
        [Required(ErrorMessage = "O tipo de combustível é obrigatório")]
        TipoCombustivel Combustivel,
        bool Disponivel,
        [Required(ErrorMessage = "O fabricante é obrigatório")]
        long FabricanteId);
}
