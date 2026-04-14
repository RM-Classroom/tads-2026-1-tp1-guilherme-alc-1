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
    public class AlugueisController : ControllerBase
    {
        private readonly ILogger<AlugueisController> _logger;
        private readonly ApplicationContext _context;
        public AlugueisController(ILogger<AlugueisController> logger, ApplicationContext context)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém a lista de todos os aluguéis.
        /// </summary>
        /// <returns>Lista de aluguéis cadastrados.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AluguelResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AluguelResponseDTO>>> GetAsync()
        {
            try
            {
                var alugueis = await _context.Alugueis
                    .AsNoTracking()
                    .Select(a => new AluguelResponseDTO
                        (
                            a.Id,
                            a.DataInicio,
                            a.DataTermino,
                            a.QuilometragemInicial,
                            a.QuilometragemFinal,
                            a.ValorDiaria,
                            a.QuantidadeDiarias,
                            a.ValorTotal,
                            a.Status,
                            a.Observacoes,
                            a.ClienteId,
                            a.VeiculoId
                        )
                    )
                    .ToListAsync();

                return Ok(alugueis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter alugueis.");
                return StatusCode(500, "Ocorreu um erro interno ao obter os alugueis");
            }
        }

        /// <summary>
        /// Obtém um aluguel pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do aluguel.</param>
        /// <returns>Os dados do aluguel encontrado.</returns>
        /// <response code="200">Aluguel encontrado com sucesso.</response>
        /// <response code="404">Aluguel não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(AluguelResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AluguelResponseDTO>> GetByIdAsync(long id)
        {
            try
            {
                var aluguel = await _context.Alugueis
                    .AsNoTracking()
                    .Where(a => a.Id == id)
                    .Select(a => new AluguelResponseDTO
                        (
                             a.Id,
                            a.DataInicio,
                            a.DataTermino,
                            a.QuilometragemInicial,
                            a.QuilometragemFinal,
                            a.ValorDiaria,
                            a.QuantidadeDiarias,
                            a.ValorTotal,
                            a.Status,
                            a.Observacoes,
                            a.ClienteId,
                            a.VeiculoId
                        )
                    )
                    .FirstOrDefaultAsync();

                if (aluguel == null)
                    return NotFound("Aluguel não encontrado.");

                return Ok(aluguel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter aluguel de Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter o aluguel");
            }
        }

        /// <summary>
        /// Obtém os aluguéis de um cliente específico.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Lista de aluguéis vinculados ao cliente informado.</returns>
        /// <response code="200">Aluguéis obtidos com sucesso.</response>
        /// <response code="404">Cliente não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("por-cliente/{id:long}")]
        [ProducesResponseType(typeof(IEnumerable<AlugueisPorClienteResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AlugueisPorClienteResponseDTO>>> GetByClienteAsync(long id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound("O cliente informado não foi encontado.");

                var alugueis = await _context.Alugueis
                    .AsNoTracking()
                    .Where(a => a.ClienteId == id)
                    .Select(a => new AlugueisPorClienteResponseDTO
                        (
                            a.Id,
                            a.DataInicio,
                            a.DataTermino,
                            a.QuilometragemInicial,
                            a.QuilometragemFinal,
                            a.ValorDiaria,
                            a.QuantidadeDiarias,
                            a.ValorTotal,
                            a.Status,
                            a.Observacoes,
                            new VeiculoResponseDTO(
                                a.Veiculo.Id,
                                a.Veiculo.Modelo,
                                a.Veiculo.Ano,
                                a.Veiculo.Quilometragem,
                                a.Veiculo.Placa,
                                a.Veiculo.Cor,
                                a.Veiculo.Combustivel,
                                a.Veiculo.Disponivel,
                                null
                            ),
                            new ClienteResponseDTO(
                                a.Cliente.Id,
                                a.Cliente.Nome,
                                a.Cliente.CPF,
                                a.Cliente.Email,
                                a.Cliente.Telefone
                            )
                        )
                    )
                    .ToListAsync();

                return Ok(alugueis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter alugueis do cliente de Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter os alugueis");
            }
        }

        /// <summary>
        /// Obtém os aluguéis filtrados por status.
        /// </summary>
        /// <param name="status">Status do aluguel utilizado no filtro.</param>
        /// <returns>Lista de aluguéis com o status informado.</returns>
        /// <response code="200">Aluguéis obtidos com sucesso.</response>
        /// <response code="400">Status do aluguel inválido.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("por-status")]
        [ProducesResponseType(typeof(IEnumerable<AlugueisPorClienteResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AlugueisPorClienteResponseDTO>>> GetByStatusAsync([FromQuery] StatusAluguel status)
        {
            try
            {
                if (!Enum.IsDefined(typeof(StatusAluguel), status))
                    return BadRequest("Status do aluguel inválido.");

                var alugueis = await _context.Alugueis
                    .AsNoTracking()
                    .Where(a => a.Status == status)
                    .Select(a => new AlugueisPorClienteResponseDTO(
                        a.Id,
                        a.DataInicio,
                        a.DataTermino,
                        a.QuilometragemInicial,
                        a.QuilometragemFinal,
                        a.ValorDiaria,
                        a.QuantidadeDiarias,
                        a.ValorTotal,
                        a.Status,
                        a.Observacoes,
                        new VeiculoResponseDTO(
                            a.Veiculo.Id,
                            a.Veiculo.Modelo,
                            a.Veiculo.Ano,
                            a.Veiculo.Quilometragem,
                            a.Veiculo.Placa,
                            a.Veiculo.Cor,
                            a.Veiculo.Combustivel,
                            a.Veiculo.Disponivel,
                            null
                        ),
                        new ClienteResponseDTO(
                            a.Cliente.Id,
                            a.Cliente.Nome,
                            a.Cliente.CPF,
                            a.Cliente.Email,
                            a.Cliente.Telefone
                        )
                    ))
                    .ToListAsync();

                return Ok(alugueis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter alugueis com status {status}.", status);
                return StatusCode(500, "Ocorreu um erro interno ao obter os alugueis.");
            }
        }

        /// <summary>
        /// Obtém um relatório consolidado dos aluguéis com dados de cliente, veículo e pagamento.
        /// </summary>
        /// <returns>Relatório de aluguéis.</returns>
        /// <response code="200">Relatório obtido com sucesso.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("relatorio")]
        [ProducesResponseType(typeof(IEnumerable<AluguelRelatorioResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AluguelRelatorioResponseDTO>>> GetRelatorioAsync()
        {
            try
            {
                var relatorio = await _context.Alugueis
                    .AsNoTracking()
                    .Select(a => new AluguelRelatorioResponseDTO(
                        a.Id,
                        a.DataInicio,
                        a.DataTermino,
                        a.ValorDiaria,
                        a.QuantidadeDiarias,
                        a.ValorTotal,
                        a.Cliente.Nome,
                        a.Veiculo.Modelo,
                        a.Veiculo.Placa,
                        a.Pagamento != null ? a.Pagamento.FormaPagamento.ToString() : null,
                        a.Pagamento != null ? a.Pagamento.Valor : null,
                        a.Pagamento != null ? a.Pagamento.DataPagamento : null,
                        a.Pagamento != null ? a.Pagamento.Status.ToString() : null
                    ))
                    .ToListAsync();

                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatório de aluguéis.");
                return StatusCode(500, "Ocorreu um erro interno ao obter o relatório de aluguéis.");
            }
        }

        /// <summary>
        /// Cria um novo aluguel.
        /// </summary>
        /// <param name="request">Dados necessários para criação do aluguel.</param>
        /// <returns>O aluguel criado.</returns>
        /// <response code="201">Aluguel criado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos ou o veículo não está disponível.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AluguelResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AluguelResponseDTO>> CreateAsync([FromBody] AluguelRequestDTO request)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(request.ClienteId);
                if (cliente == null)
                    return BadRequest("O cliente informado não foi encontado.");

                var veiculo = await _context.Veiculos.FindAsync(request.VeiculoId);
                if (veiculo == null)
                    return BadRequest("O veículo informado não foi encontrado.");

                if (!veiculo.Disponivel)
                    return BadRequest("O veículo não está disponível para locação");

                var aluguel = new Aluguel
                {
                    DataInicio = request.DataInicio,
                    DataTermino = request.DataTermino,
                    DataCriacao = DateTime.UtcNow,
                    QuilometragemInicial = veiculo.Quilometragem,
                    QuilometragemFinal = request.QuilometragemFinal,
                    ValorDiaria = request.ValorDiaria,
                    QuantidadeDiarias = request.QuantidadeDiarias,
                    Status = request.Status,
                    Observacoes = request.Observacoes,
                    ClienteId = request.ClienteId,
                    Cliente = cliente,
                    VeiculoId = request.VeiculoId,
                    Veiculo = veiculo,
                };


                await _context.Alugueis.AddAsync(aluguel);
                veiculo.Disponivel = false;
                await _context.SaveChangesAsync();

                var response = new AluguelResponseDTO(
                    aluguel.Id,
                    aluguel.DataInicio,
                    aluguel.DataTermino,
                    aluguel.QuilometragemInicial,
                    aluguel.QuilometragemFinal,
                    aluguel.ValorDiaria,
                    aluguel.QuantidadeDiarias,
                    aluguel.ValorTotal,
                    aluguel.Status,
                    aluguel.Observacoes,
                    aluguel.ClienteId,
                    aluguel.VeiculoId
                );

                return CreatedAtAction("GetById", new { id = aluguel.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aluguel.");
                return StatusCode(500, "Ocorreu um erro interno ao criar o aluguel");
            }
        }

        /// <summary>
        /// Atualiza os dados de um aluguel existente.
        /// </summary>
        /// <param name="id">Identificador do aluguel.</param>
        /// <param name="request">Novos dados do aluguel.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Aluguel atualizado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos ou o veículo não está disponível.</response>
        /// <response code="404">Aluguel não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AluguelResponseDTO>> UpdateAsync(long id, [FromBody] AluguelRequestDTO request)
        {
            try
            {
                var aluguel = await _context.Alugueis.FindAsync(id);
                if (aluguel == null)
                    return NotFound("Aluguel não encontrado.");

                var cliente = await _context.Clientes.FindAsync(request.ClienteId);
                if (cliente == null)
                    return BadRequest("O cliente informado não foi encontado.");

                var veiculo = await _context.Veiculos.FindAsync(request.VeiculoId);
                if (veiculo == null)
                    return BadRequest("O veículo informado não foi encontrado.");

                if (veiculo.Id != aluguel.VeiculoId && !veiculo.Disponivel)
                    return BadRequest("O veículo informado não está disponível para locação");

                var veiculoIdAnterior = aluguel.VeiculoId;

                aluguel.DataInicio = request.DataInicio;
                aluguel.DataTermino = request.DataTermino;
                aluguel.QuilometragemInicial = veiculo.Quilometragem;
                aluguel.QuilometragemFinal = request.QuilometragemFinal;
                aluguel.ValorDiaria = request.ValorDiaria;
                aluguel.QuantidadeDiarias = request.QuantidadeDiarias;
                aluguel.Status = request.Status;
                aluguel.Observacoes = request.Observacoes;
                aluguel.ClienteId = request.ClienteId;
                aluguel.Cliente = cliente;
                aluguel.VeiculoId = request.VeiculoId;
                aluguel.Veiculo = veiculo;

                if (veiculo.Id != veiculoIdAnterior)
                {
                    var veiculoAnterior = await _context.Veiculos.FindAsync(veiculoIdAnterior);
                    if(veiculoAnterior != null)
                        veiculoAnterior.Disponivel = true;

                    veiculo.Disponivel = false;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aluguel de Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o aluguel");
            }
        }

        /// <summary>
        /// Remove um aluguel pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do aluguel.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Aluguel removido com sucesso.</response>
        /// <response code="400">Não é possível excluir o aluguel porque ele possui pagamento associado.</response>
        /// <response code="404">Aluguel não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AluguelResponseDTO>> DeleteAsync(long id)
        {
            try
            {
                var aluguel = await _context.Alugueis.FindAsync(id);
                if (aluguel == null)
                    return NotFound("Aluguel não encontrado.");

                var pagamentoExists = await _context.Pagamentos.AnyAsync(p => p.AluguelId == id);
                if (pagamentoExists)
                    return BadRequest("Não é possível excluir o aluguel, pois ele possui pagamento associado.");

                var veiculo = await _context.Veiculos.FindAsync(aluguel.VeiculoId);
                if(veiculo != null)
                    veiculo.Disponivel = true;

                _context.Alugueis.Remove(aluguel);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover aluguel de Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao remover o aluguel");
            }
        }
    }
}