using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Statuses;

namespace MosadAPIServer.Services
{
    public class TargetService
    {
        private readonly MosadDbContext _context;

        public TargetService(MosadDbContext context)
        {
            _context = context;
        }

        public async Task<Target> CreateTarget(Target target)
        {
            target.Status = TargetStatus.Status.Alive.ToString();
            _context.Targets.Add(target);
            await _context.SaveChangesAsync();
            return target;
        }

        public async Task<List<Target>> GetAllTargets()
        {
            return await _context.Targets.ToListAsync();
        }

        public async Task<Target> UpdateLocation(int id, Location location)
        {
            var target = await _context.Targets.FirstOrDefaultAsync(t => t.Id == id);
            if (target.Location == null)
            {
                target.Location = new Location();
            }
            target.Location.X = location.X;
            target.Location.Y = location.Y;

            await CreateMissions(target);
            _context.Update(target);
            await _context.SaveChangesAsync();
            return target;
        }
        public async Task FindMissions(Target target)
        {
            var agentslist = await _context.Agents.ToArrayAsync();

            foreach(Agent agent in agentslist)
            {
                if (agent.Status == AgentStatus.Status.NotActiv.ToString())
                {
                    if (MissionService.IfMission(agent, target))
                    {
                        Mission mission = MissionService.CreateMission(agent, target); //יצירת משימה
                        _context.Missions.Add(mission); //הוספה למסד נתונים
                        _context.SaveChanges();
                    }
                }
            }
        }

        public async Task<Target> UpdateDirection(int id, string direction)
        {
            var target = await _context.Targets.FirstOrDefaultAsync(t => t.Id == id);
            LocationService.VerifyingLocation(target.Location, direction);
            _context.Update(target);
            await _context.SaveChangesAsync();
            return target;
        }
        public async Task CreateMissions(Target target)
        {
            var list = await _context.Agents.ToArrayAsync();

            foreach (Agent agent in list)
            {
                if (agent.Status == AgentStatus.Status.NotActiv.ToString())
                {
                    if (MissionService.IfMission(agent, target))
                    {
                        Mission mission = MissionService.CreateMission(agent, target); //יצירת משימה
                        _context.Missions.Add(mission); //הוספה למסד נתונים
                        _context.SaveChanges();
                    }
                }
            }
            return;
        }
    }
}
