using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP1_TADS.Data;
using TP1_TADS.DTOs;
using TP1_TADS.Entities;
using TP1_TADS.Enums;

namespace TP1_TADS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<VeiculosController> _logger;
        public VeiculosController(ApplicationContext context, ILogger<VeiculosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VeiculoResponseDTO>>> GetAsync()
        {
            try
            {
                var veiculos = await _context.Veiculos
                .AsNoTracking()
                .Select(v => new VeiculoResponseDTO(
                    v.Id,
                    v.Modelo,
                    v.Ano,
                    v.Quilometragem,
                    v.Placa,
                    v.Cor,
                    v.Combustivel,
                    v.Disponivel,
                    new FabricanteResponseDTO(v.Fabricante.Id, v.Fabricante.Nome)
                ))
                .ToListAsync();
                return Ok(veiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículos.");
                return StatusCode(500, "Ocorreu um erro interno ao obter os veiculos.");
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<VeiculoResponseDTO>> GetByIdAsync(long id)
        {
            try
            {
                var veiculo = await _context.Veiculos
                    .AsNoTracking()
                    .Where(v => v.Id == id)
                    .Select(v => new VeiculoResponseDTO(
                        v.Id,
                        v.Modelo,
                        v.Ano,
                        v.Quilometragem,
                        v.Placa,
                        v.Cor,
                        v.Combustivel,
                        v.Disponivel,
                        new FabricanteResponseDTO(v.Fabricante.Id, v.Fabricante.Nome)
                    ))
                    .FirstOrDefaultAsync();

                if (veiculo == null)
                    return NotFound($"Não foi encontrado o veículo informado.");

                return Ok(veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículo com id {Id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter o veículo.");
            }
        }

        [HttpGet]
        [Route("por-fabricante/{id:long}")]
        public async Task<ActionResult<IEnumerable<VeiculoResponseDTO>>> GetByFabricanteAsync(long id)
        {
            try
            {
                var fabricante = await _context.Fabricantes.FindAsync(id);

                if (fabricante == null)
                    return NotFound("Não foi encontrado o fabricante informado");

                var veiculos = await _context.Veiculos
                    .AsNoTracking()
                    .Where(v => v.FabricanteId == id)
                    .Select(v => new VeiculoResponseDTO(
                        v.Id,
                        v.Modelo,
                        v.Ano,
                        v.Quilometragem,
                        v.Placa,
                        v.Cor,
                        v.Combustivel,
                        v.Disponivel,
                        new FabricanteResponseDTO(v.Fabricante.Id, v.Fabricante.Nome)
                    ))
                    .ToListAsync();

                return Ok(veiculos);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículos do fabricante com id {Id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter os veículos do fabricante informado.");
            }
        }

        [HttpGet]
        [Route("disponiveis")]
        public async Task<ActionResult<IEnumerable<VeiculoResponseDTO>>> GetDisponiveisAsync()
        {
            try
            {
                var veiculos = await _context.Veiculos
                    .AsNoTracking()
                    .Where(v => v.Disponivel == true)
                    .Select(v => new VeiculoResponseDTO(
                        v.Id,
                        v.Modelo,
                        v.Ano,
                        v.Quilometragem,
                        v.Placa,
                        v.Cor,
                        v.Combustivel,
                        v.Disponivel,
                        new FabricanteResponseDTO(v.Fabricante.Id, v.Fabricante.Nome)
                    ))
                    .ToListAsync();

                return Ok(veiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículos disponiveis");
                return StatusCode(500, "Ocorreu um erro interno ao obter os veículos disponiveis.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<VeiculoResponseDTO>> CreateAsync([FromBody] VeiculoRequestDTO request)
        {
            try
            {
                var placa = request.Placa.Trim().ToUpper();
                var placaExists = await _context.Veiculos.AnyAsync(v => v.Placa == placa);
                if(placaExists)
                    return Conflict($"Já existe um veículo cadastrado com a placa {request.Placa}.");

                var fabricante = await _context.Fabricantes.FindAsync(request.FabricanteId);
                if (fabricante == null)
                    return NotFound($"Não foi possível encontrar o cadastro do fabricante informado.");

                if (!Enum.IsDefined(typeof(TipoCombustivel), request.Combustivel))
                {
                    return BadRequest("Tipo de combustível inválido.");
                }

                var veiculo = new Veiculo
                {
                    Modelo = request.Modelo,
                    Ano = request.Ano,
                    Quilometragem = request.Quilometragem,
                    Placa = request.Placa.ToUpper().Trim(),
                    Cor = request.Cor,
                    Combustivel = request.Combustivel,
                    Disponivel = request.Disponivel,
                    FabricanteId = fabricante.Id,
                    Fabricante = fabricante
                };

                _context.Veiculos.Add(veiculo);
                await _context.SaveChangesAsync();

                var response = new VeiculoResponseDTO(
                    veiculo.Id,
                    veiculo.Modelo,
                    veiculo.Ano,
                    veiculo.Quilometragem,
                    veiculo.Placa,
                    veiculo.Cor,
                    veiculo.Combustivel,
                    veiculo.Disponivel,
                    new FabricanteResponseDTO(fabricante.Id, fabricante.Nome)
                );

                return CreatedAtAction("GetById", new { id = veiculo.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar veículo com placa {Placa}.", request.Placa);
                return StatusCode(500, "Ocorreu um erro interno ao criar o veículo.");
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] VeiculoRequestDTO request)
        {
            try
            {
                var veiculo = await _context.Veiculos.FindAsync(id);
                if (veiculo == null)
                    return NotFound($"Não foi encontrado o veículo informado.");

                var placa = request.Placa.Trim().ToUpper();
                var placaExists = await _context.Veiculos.AnyAsync(v => v.Placa == placa && v.Id != id);
                if (placaExists)
                    return Conflict($"Já existe um veículo cadastrado com a placa {request.Placa}.");

                var fabricante = await _context.Fabricantes.FindAsync(request.FabricanteId);
                if (fabricante == null)
                    return NotFound($"Não foi encontrado o cadastro do fabricante informado.");

                if (!Enum.IsDefined(typeof(TipoCombustivel), request.Combustivel))
                {
                    return BadRequest("Tipo de combustível inválido.");
                }

                veiculo.Modelo = request.Modelo;
                veiculo.Ano = request.Ano;
                veiculo.Quilometragem = request.Quilometragem;
                veiculo.Placa = request.Placa.ToUpper().Trim();
                veiculo.Cor = request.Cor;
                veiculo.Combustivel = request.Combustivel;
                veiculo.Disponivel = request.Disponivel;
                veiculo.FabricanteId = fabricante.Id;
                veiculo.Fabricante = fabricante;

                _context.Veiculos.Update(veiculo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar veículo com id {Id} e placa {Placa}.", id, request.Placa);
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o veículo.");
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> DeleteAsync(long id)
        {
            try
            {
                var veiculo = await _context.Veiculos.FindAsync(id);

                if (veiculo == null)
                    return NotFound($"Não foi encontrado o cadastro do veículo informado.");

                _context.Veiculos.Remove(veiculo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir veículo com id {Id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao excluir o veículo.");
            }
        }
    }
}
