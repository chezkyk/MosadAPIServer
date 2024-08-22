using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Services;
using System.Text.Json;

namespace MosadAPIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly MosadDbContext _context;
        // יצירת משתנה של DB תוך כדי שמירה על עקרון DI
        public AgentsController(MosadDbContext context)
        {
            this._context = context;
        }
        //--Create Agent--
        [HttpPost]
        public async Task<IActionResult> CreateAgent(Agent agent)
        {
            // עדכון סטטוס סוכן שנוצר ללא פעיל
            agent.Status = AgentStatus.Status.NotActiv.ToString();
            _context.Agents.Add(agent);// הוספת הסוכן ל DB
            await _context.SaveChangesAsync();// שמירת השינויים
            return StatusCode(
            StatusCodes.Status201Created,
            new { agent = agent.Id });
        }
        //--Get All Agents
        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            // הכנסת LIST של סוכנים לתוך המשתנה agent
            var agents = await _context.Agents.ToListAsync();
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
            Agent agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == id);
            // הבדיקה הבאה נצרכת רק בפעם הראשונה שאני מעדכן את המיקום.
            if (agent.Location == null)
            {
                agent.Location = new Location();
            }
            agent.Location.X = location.X;
            agent.Location.Y = location.Y;

            var targetList = await _context.Targets.ToListAsync();

            foreach (Target target in targetList)
            {
                if (target.Status == TargetStatus.Status.Alive.ToString())
                {
                    if (MissionService.IfMission(agent, target) && await IfNotTarget(target.Id))
                    {
                        Mission mission = MissionService.CreateMission(agent, target);
                        _context.Missions.Add(mission);
                    }
                }
            }
            _context.Update(agent);
            await _context.SaveChangesAsync();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                }
            );
        }
        //--Update Direction--
        [HttpPut("{id}/move")]
        public async Task<IActionResult> UpdateDirection(int id, [FromBody] Dictionary<string, string> direction)
        {
            Agent agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == id);

            string stringDirection = direction["direction"];

            VerifyingLocation(agent, stringDirection);
            _context.Update(agent);


            await _context.SaveChangesAsync();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                }
            );
        }
        //Help function
        private async Task<bool> IfNotTarget(int? id)
        {
            Mission mission = await _context.Missions.FirstOrDefaultAsync(x => x.TargetId.Id == id);
            if (mission == null || mission.Status == MissionStatus.Status.Offer.ToString())
            {
                return true;
            }
            return false;

        }
        public void VerifyingLocation(Agent agent ,string direction)
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
        }

    }
}

