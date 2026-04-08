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
        public FabricantesController(ApplicationContext context)
        {
            _context = context;
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
                return StatusCode(500, "Ocorreu um erro interno ao obter os fabricantes." + Environment.NewLine + ex);
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
                return StatusCode(500, "Ocorreu um erro interno ao obter o fabricante." + Environment.NewLine + ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<FabricanteResponseDTO>> CreateAsync([FromBody] FabricanteRequestDTO request)
        {
            try
            {
                var nome = request.Nome.Trim().ToUpper();

                var existeFabricante = await _context.Fabricantes
                    .AnyAsync(f => f.Nome.ToUpper() == nome);

                if (existeFabricante)
                {
                    return BadRequest("Já existe um fabricante com esse nome");
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
                return StatusCode(500, "Ocorreu um erro interno ao cadastrar o fabricante." + Environment.NewLine + ex);
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

                fabricante.Nome = request.Nome;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o fabricante." + Environment.NewLine + ex);
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
                return StatusCode(500, "Ocorreu um erro interno ao remover o fabricante." + Environment.NewLine + ex);
            }
        }
    }
}