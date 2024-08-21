using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosadAPIServer.Models;
using MosadAPIServer.Services;

namespace MosadAPIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly MosadDbContext _context;

        public AgentsController(MosadDbContext context)
        {
            this._context = context;
        }
        //--Create Agent--
        [HttpPost]
        public async Task<IActionResult> CreateAgent(Agent agent)
        {
            
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();
            return StatusCode(
            StatusCodes.Status201Created,
            new { agent = agent.Id });
        }
        //--Get All Agents
        [HttpGet]
        public IActionResult GetAllAttacks()
        {

            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    attacks = _context.Agents.ToArray()
                }
            );
        }

    }
}
