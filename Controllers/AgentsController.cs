using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Services;
using MosadAPIServer.Statuses;
using System.Text.Json;

namespace MosadAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly AgentService _agentService;
        private readonly MissionService _missionService;
        private readonly MosadDbContext _context;

        public AgentsController(AgentService agentService, MissionService missionService, MosadDbContext context)
        {
            _agentService = agentService;
            _missionService = missionService;
            _context = context;
        }
        //--Create Agent--
        [HttpPost]
        public async Task<IActionResult> CreateAgent(Agent agent)
        {
            var createdAgent = await _agentService.CreateAgent(agent);
            return StatusCode(StatusCodes.Status201Created, agent);
        }
        //--Get All Agents
        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            // הכנסת LIST של סוכנים לתוך המשתנה agent
            var agents = await _agentService.GetAllAgents();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    agents = agents
                }
            );
        }
        //-- Update Location --
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> UpdateLocation(int id, Location location)
        {
            var agent = await _agentService.UpdateLocation(id, location);
            var targets = await _context.Targets.ToListAsync();

            foreach (var target in targets)
            {
                if (target.Status == TargetStatus.Status.Alive.ToString())
                {
                    if (MissionService.IfMission(agent, target) && await IfNotTarget(target.Id))
                    {
                        var mission = MissionService.CreateMission(agent, target);
                        _context.Missions.Add(mission);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        //--Update Direction--
        [HttpPut("{id}/move")]
        public async Task<IActionResult> UpdateDirection(int id, [FromBody] Dictionary<string, string> direction)
        {
            await _agentService.UpdateDirection(id, direction["direction"]);
            return Ok();
        }
        //Help function
        private async Task<bool> IfNotTarget(int? id)
        {
            var mission = await _context.Missions.FirstOrDefaultAsync(x => x.TargetId.Id == id);
            return mission == null || mission.Status == MissionStatus.Status.Offer.ToString();

        }
        

    }
}

