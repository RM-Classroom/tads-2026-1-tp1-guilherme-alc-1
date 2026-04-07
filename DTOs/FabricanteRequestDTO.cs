using System.ComponentModel.DataAnnotations;

namespace TP1_TADS.DTOs
{
    public record FabricanteRequestDTO(
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        string Nome
    );
}