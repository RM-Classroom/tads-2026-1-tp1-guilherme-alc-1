using System.ComponentModel.DataAnnotations;
using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record AluguelRequestDTO(
        [DataType(DataType.Date)]
        DateTime DataInicio,

        [DataType(DataType.Date)]
        DateTime DataTermino,

        [Range(0, int.MaxValue, ErrorMessage = "A quilometragem final deve ser maior ou igual a zero")]
        int? QuilometragemFinal,

        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "O valor da diária deve ser maior ou igual a zero")]
        decimal ValorDiaria,

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade de diárias deve ser maior que zero")]
        int QuantidadeDiarias,

        StatusAluguel Status,

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        string? Observacoes,

        [Range(1, long.MaxValue, ErrorMessage = "ClienteId deve ser válido")]
        long ClienteId,

        [Range(1, long.MaxValue, ErrorMessage = "VeiculoId deve ser válido")]
        long VeiculoId
    );
}
