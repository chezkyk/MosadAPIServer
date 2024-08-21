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

