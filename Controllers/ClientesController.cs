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

        /// <summary>
        /// Obtém a lista de todos os clientes.
        /// </summary>
        /// <returns>Lista de clientes cadastrados.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Obtém um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Os dados do cliente encontrado.</returns>
        /// <response code="200">Cliente encontrado com sucesso.</response>
        /// <response code="404">Cliente não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="request">Dados necessários para criação do cliente.</param>
        /// <returns>O cliente criado.</returns>
        /// <response code="201">Cliente criado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos.</response>
        /// <response code="409">Já existe um cliente com o CPF informado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <param name="request">Novos dados do cliente.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Cliente atualizado com sucesso.</response>
        /// <response code="404">Cliente não encontrado.</response>
        /// <response code="409">Já existe outro cliente com o CPF informado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Remove um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Cliente removido com sucesso.</response>
        /// <response code="400">Não é possível excluir o cliente porque ele possui aluguéis associados.</response>
        /// <response code="404">Cliente não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
