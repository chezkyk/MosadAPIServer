using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Statuses;

namespace MosadAPIServer.Services
{
    public class MissionService
    {
        private readonly MosadDbContext _context;

        public MissionService(MosadDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mission>> GetAllMissions()
        {
            return await _context.Missions.Include(m => m.AgentId).Include(m => m.TargetId).ToListAsync();
        }

        public async Task<Mission> UpdateMissionStatus(int id)
        {
            var mission = await _context.Missions.FirstOrDefaultAsync(m => m.Id == id);
            mission.Status = MissionStatus.Status.InMission.ToString();
            _context.Update(mission);
            await _context.SaveChangesAsync();
            return mission;
        }

        public async Task UpdateMissions()
        {
            var missions = await _context.Missions.Include(m => m.AgentId).Include(m => m.TargetId).ToListAsync();

            foreach (var mission in missions)
            {
                var agent = mission.AgentId;
                var target = mission.TargetId;

                var command = CreateCommeandForAgent(agent, target);

                if (agent.Location.X == target.Location.X && agent.Location.Y == target.Location.Y)
                {
                    target.Status = TargetStatus.Status.Dead.ToString();
                    agent.Status = AgentStatus.Status.NotActiv.ToString();
                    mission.TimeLeft = 0;
                    mission.ExecutionTime += 0.2;
                    mission.Status = MissionStatus.Status.Finish.ToString();
                    continue;
                }

                LocationService.VerifyingLocation(agent.Location, command);
                mission.TimeLeft = CalculateTimeLeft(agent, target);
                mission.ExecutionTime += 0.2;
            }

            await _context.SaveChangesAsync();
        }

        public static double CalculateTimeLeft(Agent agent, Target target)
        {
            double distance = CalculateDistance(agent.Location.X, agent.Location.Y, target.Location.X, target.Location.Y);
            double timeLeft = distance / 5;
            return timeLeft;
        }

        public static double CalculateDistance(double xa, double ya, double xt, double yt)
        {
            return Math.Sqrt(Math.Pow(xt - xa, 2) + Math.Pow(yt - ya, 2));
        }

        public static Mission CreateMission(Agent agent, Target target)
        {
            Mission mission = new Mission();
            mission.AgentId = agent;
            mission.TargetId = target;
            mission.ExecutionTime = 0;
            mission.TimeLeft = CalculateTimeLeft(agent, target);
            mission.Status = MissionStatus.Status.Offer.ToString();
            return mission;
        }

        public static bool IfMission(Agent agent, Target target)
        {
            if (agent == null || target == null || agent.Location == null || target.Location == null)
            {
                return false;
            }
            double distance = CalculateDistance(agent.Location.X, agent.Location.Y, target.Location.X, target.Location.Y);
            return distance < 200;
        }

        public static string CreateCommeandForAgent(Agent agent, Target target)
        {
            if (agent.Location.X == target.Location.Y && agent.Location.Y < target.Location.Y)
            {
                return "n";
            }
            if (agent.Location.X == target.Location.X && agent.Location.Y > target.Location.Y)
            {
                return "s";
            }
            if (agent.Location.Y == target.Location.Y && agent.Location.X < target.Location.X)
            {
                return "e";
            }
            if (agent.Location.Y == target.Location.Y && agent.Location.X > target.Location.X)
            {
                return "w";
            }
            if (agent.Location.X > target.Location.X && agent.Location.Y > target.Location.Y)
            {
                return "sw";
            }
            if (agent.Location.X < target.Location.X && agent.Location.Y < target.Location.Y)
            {
                return "ne";
            }
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