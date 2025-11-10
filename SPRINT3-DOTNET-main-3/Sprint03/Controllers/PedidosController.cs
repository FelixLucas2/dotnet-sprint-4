using Microsoft.AspNetCore.Mvc;
using Sprint03.Service;
using Sprint03.Entidades;
using Sprint03.DTO;

namespace Sprint03.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _service;
        public PedidosController(PedidoService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Pedido>> GetById(int id)
        {
            var p = await _service.GetByIdAsync(id);
            return p is null ? NotFound() : Ok(p);
        }

        [HttpGet("com-itens")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetComItens() =>
            Ok(await _service.GetAllWithItemsAsync());

        [HttpPost]
        public async Task<ActionResult<Pedido>> Create([FromBody] PedidoDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Pedido, version = "1.0" }, created);
        }
    }
}
