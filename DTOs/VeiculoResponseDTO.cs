using TP1_TADS.Enums;

namespace TP1_TADS.DTOs
{
    public record VeiculoResponseDTO(
        long Id, 
        string Modelo, 
        int Ano, 
        int Quilometragem,
        string Placa,
        string? Cor,
        TipoCombustivel Combustivel,
        bool Disponivel,
        FabricanteResponseDTO Fabricante);
}
