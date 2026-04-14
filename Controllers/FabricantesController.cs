using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP1_TADS.Data;
using TP1_TADS.DTOs;
using TP1_TADS.Entities;

namespace TP1_TADS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FabricantesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<FabricantesController> _logger;
        public FabricantesController(ApplicationContext context, ILogger<FabricantesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém a lista de todos os fabricantes.
        /// </summary>
        /// <returns>Lista de fabricantes cadastrados.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FabricanteResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<FabricanteResponseDTO>>> GetAsync()
        {
            try
            {
                var fabricantes = await _context.Fabricantes
                    .AsNoTracking()
                    .Select(f => new FabricanteResponseDTO(f.Id, f.Nome))
                    .ToListAsync();

                return Ok(fabricantes);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Erro ao obter fabricantes.");
                return StatusCode(500, "Ocorreu um erro interno ao obter os fabricantes.");
            }
        }

        /// <summary>
        /// Obtém um fabricante pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do fabricante.</param>
        /// <returns>Os dados do fabricante encontrado.</returns>
        /// <response code="200">Fabricante encontrado com sucesso.</response>
        /// <response code="404">Fabricante não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(FabricanteResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FabricanteResponseDTO>> GetByIdAsync(long id)
        {
            try
            {
                var fabricante = await _context.Fabricantes
                    .AsNoTracking()
                    .Where(f => f.Id == id)
                    .Select(f => new FabricanteResponseDTO(f.Id, f.Nome))
                    .FirstOrDefaultAsync();

                if (fabricante == null)
                    return NotFound($"Fabricante não encontrado.");

                return Ok(fabricante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter fabricante com id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter o fabricante.");
            }
        }

        /// <summary>
        /// Cria um novo fabricante.
        /// </summary>
        /// <param name="request">Dados necessários para criação do fabricante.</param>
        /// <returns>O fabricante criado.</returns>
        /// <response code="201">Fabricante criado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos.</response>
        /// <response code="409">O fabricante já existe.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(FabricanteResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FabricanteResponseDTO>> CreateAsync([FromBody] FabricanteRequestDTO request)
        {
            try
            {
                var nome = request.Nome.Trim().ToUpper();

                var existeFabricante = await _context.Fabricantes
                    .AnyAsync(f => f.Nome.ToUpper().Trim() == nome);

                if (existeFabricante)
                {
                    return Conflict("Já existe um fabricante com esse nome.");
                }

                var fabricante = new Fabricante
                {
                    Nome = request.Nome
                };

                await _context.Fabricantes.AddAsync(fabricante);
                await _context.SaveChangesAsync();

                var response = new FabricanteResponseDTO(fabricante.Id, fabricante.Nome);

                return CreatedAtAction("GetById", new { id = fabricante.Id }, response);
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar fabricante com nome {Nome}.", request.Nome);
                return StatusCode(500, "Ocorreu um erro interno ao cadastrar o fabricante.");
            }
        }

        /// <summary>
        /// Atualiza os dados de um fabricante existente.
        /// </summary>
        /// <param name="id">Identificador do fabricante.</param>
        /// <param name="request">Novos dados do fabricante.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Fabricante atualizado com sucesso.</response>
        /// <response code="404">Fabricante não encontrado.</response>
        /// <response code="409">O fabricante já existe.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] FabricanteRequestDTO request)
        {
            try
            {
                var fabricante = await _context.Fabricantes.FindAsync(id);

                if(fabricante == null)
                    return NotFound($"Fabricante não encontrado.");

                var nome = request.Nome.Trim().ToUpper();
                var existeFabricante = await _context.Fabricantes
                    .AnyAsync(f => f.Nome.ToUpper().Trim() == nome && f.Id != id);

                if (existeFabricante)
                {
                    return Conflict("Já existe um fabricante com esse nome.");
                }

                fabricante.Nome = nome;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar fabricante com id {Id} e nome {Nome}.", id, request.Nome);
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o fabricante.");
            }
        }

        /// <summary>
        /// Remove um fabricante pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do fabricante.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Fabricante removido com sucesso.</response>
        /// <response code="404">Fabricante não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var fabricante = await _context.Fabricantes.FindAsync(id);

                if (fabricante == null)
                    return NotFound($"Fabricante não encontrado.");

                _context.Fabricantes.Remove(fabricante); 
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover fabricante com id {Id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao remover o fabricante.");
            }
        }
    }
}