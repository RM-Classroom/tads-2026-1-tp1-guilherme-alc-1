using System.ComponentModel.DataAnnotations;
using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record PagamentoRequestDTO(
        decimal Valor,
        DateTime? DataPagamento,
        StatusPagamento Status,
        FormaPagamento FormaPagamento,
        [Range(1, long.MaxValue, ErrorMessage = "AluguelId deve ser válido.")]
        long AluguelId
    );
}
