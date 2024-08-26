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
            return StatusCode(StatusCodes.Status200OK, agents);
        }
        //-- Update Location --
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> UpdateLocation(int id,[FromBody] Location location)
        {
            var agent = await _agentService.UpdateLocation(id, location);
            await _agentService.FindMissions(agent);

            await _context.SaveChangesAsync();
            return Ok();
        }
        //--Update Direction--
        [HttpPut("{id}/move")]
        public async Task<IActionResult> UpdateDirection(int id, [FromBody] Dictionary<string, string> direction)
        {
            // למשתנה agent ייכנס הסוכן הספציפי עם המזהה שהגיע בחתימת הפונקציה
            var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == id);
            await _agentService.UpdateDirection(agent, direction["direction"]);
            await _agentService.FindMissions(agent);
            return Ok();
        }

        //returns how active many agents is
        [HttpGet("SumOfActiveAgents")]
        public async Task<IActionResult> SumOfActiveAgents()
        {
            var activeAgentsCount = await _context.Agents.Where(agent => agent.Status == AgentStatus.Status.Active.ToString()).CountAsync();
            return Ok(activeAgentsCount);
        }
        //returns how not active many agents is
        [HttpGet("SumOfNotActiveAgents")]
        public async Task<IActionResult> SumOfNotActiveAgents()
        {
            var notActiveAgentsCount = await _context.Agents.Where(agent => agent.Status == AgentStatus.Status.NotActiv.ToString()).CountAsync();
            return Ok(notActiveAgentsCount);
        }
        //
        [HttpGet("SumOfGoodActiveAgents")]
        public async Task<IActionResult> SumOfGoodActiveAgents()
        {
            var notActiveAgents = await _context.Agents
                .Where(agent => agent.Status == AgentStatus.Status.NotActiv.ToString())
                .ToListAsync();
            var targetsList = await _context.Targets.ToListAsync();
            var goodTargetscount = 0;
            List<Agent> goodActiveList = new List<Agent>();
            for (int i = 0; i < notActiveAgents.Count(); i++)
            {
                foreach (var item in targetsList)
                {
                    if (MissionService.IfMission(notActiveAgents[i], item) && await _agentService.IfNotTarget(item.Id))
                    {

                        goodTargetscount++;
                    }
                }
                if (goodTargetscount != 0)
                {

                    goodActiveList.Add(notActiveAgents[i]);
                }
            }
            return StatusCode(200, new { goodActiveList = goodActiveList.Count(), goodTargetscount = goodTargetscount });
        }




        //Help function
        
    }
}




