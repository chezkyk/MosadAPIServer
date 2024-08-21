using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosadAPIServer.Models;
using MosadAPIServer.Services;

namespace MosadAPIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly MosadDbContext _context;

        public TargetsController(MosadDbContext context)
        {
            this._context = context;
        }
        //--Create Target--
        [HttpPost]
        public async Task<IActionResult> CreateTarget(Target target)
        {
            
            _context.Targets.Add(target);
            await _context.SaveChangesAsync();
            return StatusCode(
            StatusCodes.Status201Created,
            new { target = target.Id });
        }
    }
}
