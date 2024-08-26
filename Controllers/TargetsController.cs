using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Services;
using MosadAPIServer.Statuses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MosadAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly TargetService _targetService;
        private readonly MissionService _missionService;
        private readonly MosadDbContext _context;

        public TargetsController(TargetService targetService, MissionService missionService, MosadDbContext context)
        {
            this._targetService = targetService;
            this._missionService = missionService;
            this._context = context;
        }
        //--Create Target--
        [HttpPost]
        public async Task<IActionResult> CreateTarget(Target target)
        {
            var createdTarget = await _targetService.CreateTarget(target);
            return StatusCode(StatusCodes.Status201Created, target);
        }
        // --Get all targets--
        [HttpGet]
        public async Task<IActionResult> GetAllTargets()
        {
            var targets = await _targetService.GetAllTargets();
            return StatusCode(StatusCodes.Status200OK, targets);
        }
        //-- Update Location --
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> UpdateLocation(int id, Location location)
        {
            var target = await _targetService.UpdateLocation(id, location);
            _targetService.FindMissions(target);
           
            return Ok();
        }
        //--Update Direction--
        [HttpPut("{id}/move")]
        public async Task<IActionResult> UpdateDirection(int id, [FromBody] Dictionary<string, string> direction)
        {
            var target = await _context.Targets.FirstOrDefaultAsync(t => t.Id == id);
            await _targetService.UpdateDirection(id, direction["direction"]);
            await _targetService.FindMissions(target);
            return Ok();

        }
        //returns how alive targets is
        [HttpGet("SumOfAliveTargets")]
        public async Task<IActionResult> SumOfAliveTargets()
        {
            var aliveTargetsCount = await _context.Targets.Where(target => target.Status == TargetStatus.Status.Alive.ToString()).CountAsync();
            return Ok(aliveTargetsCount);
        }
        //returns how dead targets is
        [HttpGet("SumOfDeadTargets")]
        public async Task<IActionResult> SumOfDeadTargets()
        {
            var deadTargetsCount = await _context.Targets.Where(target => target.Status == TargetStatus.Status.Dead.ToString()).CountAsync();
            return Ok(deadTargetsCount);
        }
    }
}
