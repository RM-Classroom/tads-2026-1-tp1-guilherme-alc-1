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
    public class PagamentosController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<PagamentosController> _logger;

        public PagamentosController(ApplicationContext context, ILogger<PagamentosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém a lista de todos os pagamentos.
        /// </summary>
        /// <returns>Lista de pagamentos cadastrados.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PagamentoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PagamentoResponseDTO>>> GetAsync()
        {
            try
            {
                var pagamentos = await _context.Pagamentos
                    .AsNoTracking()
                    .Select(p => new PagamentoResponseDTO
                        (
                            p.Id,
                            p.Valor,
                            p.DataPagamento,
                            p.Status,
                            p.FormaPagamento,
                            p.AluguelId
                        )
                    )
                    .ToListAsync();

                return Ok(pagamentos);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pagamentos.");
                return StatusCode(500, "Ocorreu um erro interno ao obter os pagamentos.");
            }
        }

        /// <summary>
        /// Obtém um pagamento pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do pagamento.</param>
        /// <returns>Os dados do pagamento encontrado.</returns>
        /// <response code="200">Pagamento encontrado com sucesso.</response>
        /// <response code="404">Pagamento não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PagamentoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagamentoResponseDTO>> GetByIdAsync(long id)
        {
            try
            {
                var pagamento = await _context.Pagamentos
                    .AsNoTracking()
                    .Where(p => p.Id == id)
                    .Select(p => new PagamentoResponseDTO
                        (
                            p.Id,
                            p.Valor,
                            p.DataPagamento,
                            p.Status,
                            p.FormaPagamento,
                            p.AluguelId
                        )
                    )
                    .FirstOrDefaultAsync();

                if (pagamento == null)
                    return NotFound($"Pagamento não encontrado.");

                return Ok(pagamento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pagamento de Id {id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao obter o pagamento.");
            }
        }

        /// <summary>
        /// Obtém os pagamentos filtrados pela forma de pagamento.
        /// </summary>
        /// <param name="forma">Forma de pagamento utilizada no filtro.</param>
        /// <returns>Lista de pagamentos com a forma informada.</returns>
        /// <response code="200">Pagamentos obtidos com sucesso.</response>
        /// <response code="400">Forma de pagamento inválida.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpGet("por-forma")]
        [ProducesResponseType(typeof(IEnumerable<PagamentosPorFormaResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PagamentosPorFormaResponseDTO>>> GetByFormaAsync([FromQuery] FormaPagamento forma)
        {
            try
            {
                if (!Enum.IsDefined(typeof(FormaPagamento), forma))
                    return BadRequest("Forma de pagamento inválida.");

                var pagamentos = await _context.Pagamentos
                    .AsNoTracking()
                    .Where(p => p.FormaPagamento == forma)
                    .Select(p => new PagamentosPorFormaResponseDTO(
                        p.Id,
                        p.Valor,
                        p.DataPagamento,
                        p.Status.ToString(),
                        p.FormaPagamento.ToString(),
                        p.AluguelId,
                        p.Aluguel.DataInicio,
                        p.Aluguel.DataTermino
                    ))
                    .ToListAsync();

                return Ok(pagamentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pagamentos por forma {forma}.", forma);
                return StatusCode(500, "Ocorreu um erro interno ao obter os pagamentos.");
            }
        }

        /// <summary>
        /// Cria um novo pagamento.
        /// </summary>
        /// <param name="request">Dados necessários para criação do pagamento.</param>
        /// <returns>O pagamento criado.</returns>
        /// <response code="201">Pagamento criado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos.</response>
        /// <response code="404">Aluguel não encontrado.</response>
        /// <response code="409">Já existe pagamento associado ao aluguel informado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PagamentoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagamentoResponseDTO>> CreateAsync([FromBody] PagamentoRequestDTO request)
        {
            try
            {
                var aluguel = await _context.Alugueis.FindAsync(request.AluguelId);
                if (aluguel == null)
                    return NotFound($"Aluguel não encontrado.");

                var pagamentoExiste = await _context.Pagamentos
                    .AnyAsync(p => p.AluguelId == request.AluguelId);

                if (pagamentoExiste)
                    return Conflict("Já existe pagamento associado a este aluguel.");

                if (!Enum.IsDefined(typeof(FormaPagamento), request.FormaPagamento))
                {
                    return BadRequest("Forma de pagamento inválida.");
                }

                if (!Enum.IsDefined(typeof(StatusPagamento), request.Status))
                {
                    return BadRequest("Status do pagamento informado é inválido.");
                }

                if (request.Status == StatusPagamento.Pago && request.DataPagamento == null)
                    return BadRequest("Pagamento com status Pago deve informar DataPagamento.");

                if (request.Status != StatusPagamento.Pago && request.DataPagamento != null)
                    return BadRequest("Somente pagamentos concluídos devem informar DataPagamento.");

                if (request.Valor <= 0)
                    return BadRequest("O valor do pagamento deve ser maior que zero.");

                var pagamento = new Pagamento
                {
                    Valor = request.Valor,
                    DataPagamento = request.DataPagamento,
                    DataCriacao = DateTime.UtcNow,
                    Status = request.Status,
                    FormaPagamento = request.FormaPagamento,
                    AluguelId = request.AluguelId,
                    Aluguel = aluguel
                };

                _context.Pagamentos.Add(pagamento);
                await _context.SaveChangesAsync();

                var pagamentoResponseDTO = new PagamentoResponseDTO
                (
                    pagamento.Id,
                    pagamento.Valor,
                    pagamento.DataPagamento,
                    pagamento.Status,
                    pagamento.FormaPagamento,
                    pagamento.AluguelId
                );
                return CreatedAtAction("GetById", new { id = pagamento.Id }, pagamentoResponseDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pagamento.");
                return StatusCode(500, "Ocorreu um erro interno ao criar o pagamento.");
            }
        }

        /// <summary>
        /// Atualiza os dados de um pagamento existente.
        /// </summary>
        /// <param name="id">Identificador do pagamento.</param>
        /// <param name="request">Novos dados do pagamento.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Pagamento atualizado com sucesso.</response>
        /// <response code="400">Os dados informados são inválidos.</response>
        /// <response code="404">Pagamento ou aluguel não encontrado.</response>
        /// <response code="409">Já existe outro pagamento associado ao aluguel informado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagamentoResponseDTO>> UpdateAsync(long id, [FromBody] PagamentoRequestDTO request)
        {
            try
            {
                var pagamento = await _context.Pagamentos.FindAsync(id);
                if (pagamento == null)
                    return NotFound("Pagamento não encontrado.");

                var aluguel = await _context.Alugueis.FindAsync(request.AluguelId);
                if (aluguel == null)
                    return NotFound($"Aluguel não encontrado.");

                var pagamentoExiste = await _context.Pagamentos
                    .AnyAsync(p => p.AluguelId == request.AluguelId && p.Id != id);

                if (pagamentoExiste)
                    return Conflict("Já existe pagamento associado a este aluguel.");

                if (!Enum.IsDefined(typeof(FormaPagamento), request.FormaPagamento))
                {
                    return BadRequest("Forma de pagamento inválida.");
                }

                if (!Enum.IsDefined(typeof(StatusPagamento), request.Status))
                {
                    return BadRequest("Status do pagamento informado é inválido.");
                }

                if (request.Status == StatusPagamento.Pago && request.DataPagamento == null)
                    return BadRequest("Pagamento com status Pago deve informar DataPagamento.");

                if (request.Status != StatusPagamento.Pago && request.DataPagamento != null)
                    return BadRequest("Somente pagamentos concluídos devem informar DataPagamento.");

                if (request.Valor <= 0)
                    return BadRequest("O valor do pagamento deve ser maior que zero.");

                pagamento.Valor = request.Valor;
                pagamento.DataPagamento = request.DataPagamento;
                pagamento.Status = request.Status;
                pagamento.FormaPagamento = request.FormaPagamento;
                pagamento.AluguelId = request.AluguelId;
                pagamento.Aluguel = aluguel;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pagamento de Id {id}", id);
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o pagamento.");
            }
        }

        /// <summary>
        /// Remove um pagamento pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do pagamento.</param>
        /// <returns>Retorna sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Pagamento removido com sucesso.</response>
        /// <response code="404">Pagamento não encontrado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var pagamento = await _context.Pagamentos.FindAsync(id);
                if (pagamento == null)
                    return NotFound("Pagamento não encontrado.");

                _context.Pagamentos.Remove(pagamento);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover pagamento de Id {id}", id);
                return StatusCode(500, "Ocorreu um erro interno ao remover o pagamento.");
            }
        }
    }
}
