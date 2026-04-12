namespace TP1_TADS.DTOs
{
    public record PagamentosPorFormaResponseDTO(
        long Id,
        decimal Valor,
        DateTime? DataPagamento,
        string Status,
        string FormaPagamento,
        long AluguelId,
        DateTime DataInicioAluguel,
        DateTime DataTerminoAluguel
    );
}