using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP1_TADS.Data;
using TP1_TADS.Entities;
using TP1_TADS.DTOs;

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

        [HttpGet]
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

        [HttpGet("{id:long}")]
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
                    return NotFound($"Não foi encontrado fabricante com id {id} cadastrado");

                return Ok(fabricante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter fabricante com id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter o fabricante.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FabricanteResponseDTO>> CreateAsync([FromBody] FabricanteRequestDTO request)
        {
            try
            {
                var nome = request.Nome.Trim().ToUpper();

                var existeFabricante = await _context.Fabricantes
                    .AnyAsync(f => f.Nome.ToUpper().Trim() == nome);

                if (existeFabricante)
                {
                    return Conflict("Já existe um fabricante com esse nome");
                }

                var fabricante = new Fabricante
                {
                    Nome = request.Nome.Trim()
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

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] FabricanteRequestDTO request)
        {
            try
            {
                var fabricante = await _context.Fabricantes.FindAsync(id);

                if(fabricante == null)
                    return NotFound($"Não foi encontrado fabricante com id {id} cadastrado");

                var nome = request.Nome.Trim().ToUpper();
                var existeFabricante = await _context.Fabricantes
                    .AnyAsync(f => f.Nome.ToUpper().Trim() == nome && f.Id != id);

                if (existeFabricante)
                {
                    return Conflict("Já existe um fabricante com esse nome");
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

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var fabricante = await _context.Fabricantes.FindAsync(id);

                if (fabricante == null)
                    return NotFound($"Não foi encontrado fabricante com id {id} cadastrado");

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