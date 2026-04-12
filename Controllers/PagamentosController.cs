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

        [HttpGet]
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

        [HttpGet("{id:long}")]
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

        [HttpPost]
        public async Task<ActionResult<PagamentoResponseDTO>> CreateAsync([FromBody] PagamentoRequestDTO request)
        {
            try
            {
                var aluguel = await _context.Alugueis.FindAsync(request.AluguelId);
                if (aluguel == null)
                    return BadRequest($"Aluguel não encontrado.");

                var pagamentoExiste = await _context.Pagamentos
                    .AnyAsync(p => p.AluguelId == request.AluguelId);

                if (pagamentoExiste)
                    return BadRequest("Já existe pagamento associado a este aluguel.");

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

        [HttpPut("{id:long}")]
        public async Task<ActionResult<PagamentoResponseDTO>> UpdateAsync(long id, [FromBody] PagamentoRequestDTO request)
        {
            try
            {
                var pagamento = await _context.Pagamentos.FindAsync(id);
                if (pagamento == null)
                    return NotFound("Pagamento não encontrado.");

                var aluguel = await _context.Alugueis.FindAsync(request.AluguelId);
                if (aluguel == null)
                    return BadRequest($"Aluguel não encontrado.");

                var pagamentoExiste = await _context.Pagamentos
                    .AnyAsync(p => p.AluguelId == request.AluguelId && p.Id != id);

                if (pagamentoExiste)
                    return BadRequest("Já existe pagamento associado a este aluguel.");

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

        [HttpDelete("{id:long}")]
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
