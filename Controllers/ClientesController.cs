using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP1_TADS.Data;
using TP1_TADS.DTOs;

namespace TP1_TADS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<ClientesController> _logger;
        public ClientesController(ApplicationContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteResponseDTO>>> GetAsync()
        {
            try
            {
                var clientes = await _context.Clientes
                    .AsNoTracking()
                    .Select(c => new ClienteResponseDTO(c.Id, c.Nome, c.CPF, c.Email, c.Telefone))
                    .ToListAsync();

                return Ok(clientes);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Erro ao obter clientes.");
                return StatusCode(500, "Ocorreu um erro interno ao obter os clientes.");
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ClienteResponseDTO>> GetByIdAsync(long id)
        {
            try
            {
                var cliente = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Id == id)
                    .Select(c => new ClienteResponseDTO(c.Id, c.Nome, c.CPF, c.Email, c.Telefone))
                    .FirstOrDefaultAsync();

                if (cliente == null)
                    return NotFound("Cliente não encontrado.");

                return Ok(cliente);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Erro ao obter cliente com Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter o cliente.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClienteResponseDTO>> CreateAsync([FromBody] ClienteRequestDTO request)
        {
            try
            {
                var cpfExists = await _context.Clientes.AnyAsync(c => c.CPF == request.CPF);

                if (cpfExists)
                    return Conflict("Já existe um cliente com o CPF informado.");

                var cliente = new Entities.Cliente
                {
                    Nome = request.Nome,
                    CPF = request.CPF,
                    Email = request.Email,
                    Telefone = request.Telefone
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                var clienteResponse = new ClienteResponseDTO(cliente.Id, cliente.Nome, cliente.CPF, cliente.Email, cliente.Telefone);
                return CreatedAtAction("GetById", new { id = cliente.Id }, clienteResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente.");
                return StatusCode(500, "Ocorreu um erro interno ao criar o cliente.");
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] ClienteRequestDTO request)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound("Cliente não encontrado.");

                var cpfExists = await _context.Clientes.AnyAsync(c => c.CPF == request.CPF && c.Id != id);
                if (cpfExists)
                    return Conflict("Já existe um cliente com o CPF informado.");

                cliente.Nome = request.Nome;
                cliente.CPF = request.CPF;
                cliente.Email = request.Email;
                cliente.Telefone = request.Telefone;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente com Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o cliente.");
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound("Cliente não encontrado.");

                var aluguelExists = await _context.Alugueis.AnyAsync(a => a.ClienteId == id);
                if (aluguelExists)
                    return BadRequest("Não é possível excluir o cliente, pois ele possui aluguéis associados.");

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente com Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao excluir o cliente.");
            }
        }
    }
}
