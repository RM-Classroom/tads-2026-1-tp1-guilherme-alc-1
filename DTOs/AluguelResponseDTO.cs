using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record AluguelResponseDTO(
        long Id, 
        DateTime DataInicio, 
        DateTime DataTermino, 
        int QuilometragemInicial, 
        int? QuilometragemFinal,
        decimal ValorDiaria,
        int QuantidadeDiarias,
        decimal ValorTotal,
        StatusAluguel Status,
        string? Observacoes,
        long ClienteId,
        long VeiculoId);
}
