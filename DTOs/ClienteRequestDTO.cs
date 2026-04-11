using System.ComponentModel.DataAnnotations;

namespace TP1_TADS.DTOs
{
    public record ClienteRequestDTO(
        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(180, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 180 caracteres.")]
        string Nome, 
        [Required(ErrorMessage = "O CPF do cliente é obrigatório.")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF deve estar no formato 000.000.000-00")]
        string CPF, 
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        string? Email,
        [Required(ErrorMessage = "O telefone do cliente é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\)\s\d{5}-\d{4}$", ErrorMessage = "Telefone deve estar no formato (XX) 99999-9999")]
        string Telefone);
}