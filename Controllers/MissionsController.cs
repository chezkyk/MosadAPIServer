using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Services;
using System.Diagnostics.Metrics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MosadAPIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionsController : ControllerBase
    {
        private readonly MosadDbContext _context;

        public MissionsController(MosadDbContext context)
        {
            this._context = context;
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update()
        {
            var missions = await _context.Missions.ToListAsync();

            foreach (var mission in missions)
            {
                var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == mission.AgentId.Id);
                var target = await _context.Targets.FirstOrDefaultAsync(t => t.Id == mission.TargetId.Id);

                if (agent == null || target == null)
                {
                    return BadRequest("Agent or Target not found.");
                }

                var command = CreateCommeandForAgent(agent, target);

                // Check if the agent has reached the target location
                if (agent.Location.X == target.Location.X && agent.Location.Y == target.Location.Y)
                {
                    target.Status = TargetStatus.Status.Dead.ToString();
                    agent.Status = AgentStatus.Status.NotActiv.ToString();
                    mission.TimeLeft = 0;
                    mission.ExecutionTime += 0.2;
                    mission.Status = MissionStatus.Status.Finish.ToString();
                    break;
                }

                agent = VerifyingLocation(agent, command);
                mission.TimeLeft = MissionService.CalculateTimeLeft(agent, target);
                mission.ExecutionTime += 0.2;
            }

            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { });
        }


        private static Agent VerifyingLocation(Agent agent, string direction)
        {
            switch (direction)
            {
                case "nw":
                    agent.Location.X -= 1;
                    agent.Location.Y -= 1;
                    break;
                case "n":
                    agent.Location.X -= 1;
                    break;
                case "ne":
                    agent.Location.X -= 1;
                    agent.Location.Y += 1;
                    break;
                case "w":
                    agent.Location.Y -= 1;
                    break;
                case "e":
                    agent.Location.Y += 1;
                    break;
                case "sw":
                    agent.Location.X += 1;
                    agent.Location.Y -= 1;
                    break;
                case "s":
                    agent.Location.X += 1;
                    break;
                case "se":
                    agent.Location.X += 1;
                    agent.Location.Y += 1;
                    break;
            }
            return agent;
        }
        //--Get All Missions
        [HttpGet]
        public async Task<IActionResult> GetAllMissions()
        {
            // הכנסת LIST של סוכנים לתוך המשתנה agent
            var missions = await _context.Missions.ToListAsync();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    missions = missions
                }
            );
        }
        //-- Update status
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, StatusJson status)
        {
            Mission mission = await _context.Missions.FirstOrDefaultAsync(m => m.Id == id);
            mission.Status = status.ToString();
            _context.Update(mission);
            await _context.SaveChangesAsync();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                }
            );


        }
        //
        public static string CreateCommeandForAgent(Agent agent, Target target)
        {
            //
            if (agent.Location.X == target.Location.Y && agent.Location.Y < target.Location.Y)
            {
                return "n";
            }
            if (agent.Location.X == target.Location.X && agent.Location.Y > target.Location.Y)
            {
                return "s";
            }
            //
            if (agent.Location.Y == target.Location.Y && agent.Location.X < target.Location.X)
            {
                return "e";
            }
            if (agent.Location.Y == target.Location.Y && agent.Location.X > target.Location.X)
            {
                return "w";
            }
            //
            if (agent.Location.X > target.Location.X && agent.Location.Y > target.Location.Y)
            {
                return "sw";
            }
            if (agent.Location.X < target.Location.X && agent.Location.Y < target.Location.Y)
            {
                return "ne";
            }
            //
            if (agent.Location.X < target.Location.X && agent.Location.Y > target.Location.Y)
            {
                return "se";
            }
            if (agent.Location.X > target.Location.X && agent.Location.Y < target.Location.Y)
            {
                return "nw";
            }
            return "";
        }

    }
}










