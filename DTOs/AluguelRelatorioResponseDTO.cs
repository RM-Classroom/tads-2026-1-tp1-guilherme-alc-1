namespace TP1_TADS.DTOs
{
    public record AluguelRelatorioResponseDTO(
        long Id,
        DateTime DataInicio,
        DateTime DataTermino,
        decimal ValorDiaria,
        int QuantidadeDiarias,
        decimal ValorTotal,
        string ClienteNome,
        string VeiculoModelo,
        string VeiculoPlaca,
        string? FormaPagamento,
        decimal? ValorPago,
        DateTime? DataPagamento,
        string? StatusPagamento
    );
}