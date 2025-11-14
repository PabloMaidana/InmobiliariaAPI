using Inmobiliaria.Api.Data;
using Inmobiliaria.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagosController : ControllerBase {
    private readonly AppDbContext _db;
    public PagosController(AppDbContext db) { _db = db; }

    // GET /api/pagos/contrato/{id}
    [HttpGet("contrato/{id:int}")]
    public async Task<ActionResult<IEnumerable<PagoDto>>> GetByContrato(int id) {
        var items = await _db.Pagos
            .Include(p => p.Contrato)
            .Where(p => p.IdContrato == id)
            .ToListAsync();

        return Ok(items.Select(PagoDto.FromEntity));
    }
}