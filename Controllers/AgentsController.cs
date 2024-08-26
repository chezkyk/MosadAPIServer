﻿using Microsoft.AspNetCore.Http;
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
                    if (MissionService.IfMission(notActiveAgents[i], item) && await IfNotTarget(item.Id))
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
        private async Task<bool> IfNotTarget(int? id)
        {
            var mission = await _context.Missions.FirstOrDefaultAsync(x => x.TargetId.Id == id);
            return mission == null || mission.Status == MissionStatus.Status.Offer.ToString();

        }
    }
}




