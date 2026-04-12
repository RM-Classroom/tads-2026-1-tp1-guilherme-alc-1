using System.ComponentModel.DataAnnotations;
using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record PagamentoRequestDTO(
        [Required(ErrorMessage = "O valor do pagamento é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do pagamento deve ser maior que zero.")]
        decimal Valor,
        DateTime? DataPagamento,
        [Required(ErrorMessage = "O status do pagamento é obrigatório.")]
        StatusPagamento Status,
        [Required(ErrorMessage = "A forma de pagamento é obrigatória.")]
        FormaPagamento FormaPagamento,
        [Required(ErrorMessage = "O Id do aluguel é obrigatório.")]
        long AluguelId
    );
}
