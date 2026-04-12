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

        [HttpGet]
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

        [HttpGet("{id:long}")]
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

        [HttpGet]
        [Route("por-cliente/{id:long}")]
        public async Task<ActionResult<IEnumerable<AlugueisPorClienteResponseDTO>>> GetByClienteAsync(long id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return BadRequest("O cliente informado não foi encontado.");

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

        [HttpGet("por-status")]
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

        [HttpGet("relatorio")]
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

        [HttpPost]
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

        [HttpPut("{id:long}")]
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

        [HttpDelete("{id:long}")]
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