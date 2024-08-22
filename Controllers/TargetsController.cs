using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        // --Get all targets--
        public IActionResult GetAllTargets()
        {

            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    targets = _context.Targets.ToArray()// בהמשך לשנות לפי דרישה
                }
            );
        }
        //-- Update Location --
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> UpdateLocation(int id, Location location)
        {
            Target target = await _context.Targets.FirstOrDefaultAsync(t => t.Id == id);
            // הבדיקה הבאה נצרכת רק בפעם הראשונה שאני מעדכן את המיקום.
            if (target.Location == null)
            {
                target.Location = new Location();
            }
            target.Location.X = location.X;
            target.Location.Y = location.Y;
            _context.Update(target);
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
            Target target = await _context.Targets.FirstOrDefaultAsync(t => t.Id == id);

            string stringDirection = direction["direction"];

            VerifyingLocation(target, stringDirection);
            _context.Update(target);


            await _context.SaveChangesAsync();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                }
            );
        }
        //Help function
        public void VerifyingLocation(Target target, string direction)
        {
            switch (direction)
            {
                case "nw":
                    target.Location.X -= 1;
                    target.Location.Y -= 1;
                    break;
                case "n":
                    target.Location.X -= 1;
                    break;
                case "ne":
                    target.Location.X -= 1;
                    target.Location.Y += 1;
                    break;
                case "w":
                    target.Location.Y -= 1;
                    break;
                case "e":
                    target.Location.Y += 1;
                    break;
                case "sw":
                    target.Location.X += 1;
                    target.Location.Y -= 1;
                    break;
                case "s":
                    target.Location.X += 1;
                    break;
                case "se":
                    target.Location.X += 1;
                    target.Location.Y += 1;
                    break;
            }
        }
    }
}
