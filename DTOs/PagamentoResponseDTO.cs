using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record PagamentoResponseDTO(long Id, decimal Valor, DateTime? DataPagamento, StatusPagamento Status, FormaPagamento FormaPagamento, long AluguelId);
}