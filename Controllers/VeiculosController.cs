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

        /// <summary>
        /// Obtém a lista de todos os veículos.
        /// </summary>
        /// <returns>Lista de veículos cadastrados.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VeiculoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Obtém um veículo pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <returns>Os dados do veículo encontrado.</returns>
        /// <response code="200">Veículo encontrado com sucesso.</response>
        /// <response code="404">Veículo não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(VeiculoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Obtém os veículos de um fabricante específico.
        /// </summary>
        /// <param name="id">Identificador do fabricante.</param>
        /// <returns>Lista de veículos do fabricante informado.</returns>
        /// <response code="200">Veículos obtidos com sucesso.</response>
        /// <response code="404">Fabricante não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("por-fabricante/{id:long}")]
        [ProducesResponseType(typeof(IEnumerable<VeiculoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Obtém a lista de veículos disponíveis para locação.
        /// </summary>
        /// <returns>Lista de veículos disponíveis.</returns>
        /// <response code="200">Veículos obtidos com sucesso.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("disponiveis")]
        [ProducesResponseType(typeof(IEnumerable<VeiculoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Cria um novo veículo.
        /// </summary>
        /// <param name="request">Dados necessários para criação do veículo.</param>
        /// <returns>O veículo criado.</returns>
        /// <response code="201">Veículo criado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos.</response>
        /// <response code="404">Fabricante não encontrado.</response>
        /// <response code="409">Já existe um veículo com a placa informada.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(VeiculoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Atualiza os dados de um veículo existente.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <param name="request">Novos dados do veículo.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Veículo atualizado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos.</response>
        /// <response code="404">Veículo ou fabricante não encontrado.</response>
        /// <response code="409">Já existe outro veículo com a placa informada.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Remove um veículo pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Veículo removido com sucesso.</response>
        /// <response code="400">Não é possível excluir o veículo porque ele possui aluguéis associados.</response>
        /// <response code="404">Veículo não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(long id)
        {
            try
            {
                var veiculo = await _context.Veiculos.FindAsync(id);

                if (veiculo == null)
                    return NotFound($"Não foi encontrado o cadastro do veículo informado.");

                var aluguelExists = await _context.Alugueis.AnyAsync(a => a.VeiculoId == id);
                if (aluguelExists)
                    return BadRequest("Não é possível excluir o veículo, pois ele possui aluguéis associados.");

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
