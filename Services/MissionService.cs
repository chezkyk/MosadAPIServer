using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Controllers;
using MosadAPIServer.Models;
using MosadAPIServer.Statuses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Reflection;

namespace MosadAPIServer.Services
{
    public class MissionService
    {
        private readonly MosadDbContext _context;
        private readonly AgentService _agentService;
        private readonly LocationService _locationService;
        public MissionService(MosadDbContext context, AgentService agentService, LocationService locationService)
        {
            _context = context;
            _agentService = agentService;
            _locationService = locationService;
        }

        // פונקציה זאת מחזירה רת כל המימות
        public async Task<List<Mission>> GetAllMissions()
        {
            return await _context.Missions.Include(m => m.AgentId).Include(m => m.TargetId).ToListAsync();
        }


        // פונקציה זאת מעדכנת סטטוס מתאים
        public async Task<Mission> UpdateMissionStatus(int id)
        {
            // שליפת מה DB את כל המשימות כולל המזהה של סוכן ומטרה
            var mission = await _context.Missions.Include(m => m.AgentId).Include(m => m.TargetId).FirstOrDefaultAsync(m => m.Id == id);
            // בדיקת כמה זמן נשאר
            mission.TimeLeft = CalculateTimeLeft(mission.AgentId, mission.TargetId);

            // בדיקה אם הסוכן קרוב מספיק
            if (IfMission(mission.AgentId, mission.TargetId))
            {
                //עדכון הסטטוס של משימה
                mission.Status = MissionStatus.Status.InMission.ToString();
                _context.Update(mission);
                await _context.SaveChangesAsync();

                // עדכון סטטוס של סוכן
                mission.AgentId.Status = AgentStatus.Status.Active.ToString();
                _context.Update(mission);
                await _context.SaveChangesAsync();
                // קריאה לפונקציה שתסיר את כל שאר ההצעוות למשימה לאותו סוכן ומטרה
                RemoveMission(mission);


                await _context.SaveChangesAsync();
                return mission;
            }
            else
            {
                
                _context.Missions.Remove(mission);
                _context.Update(mission);
                await _context.SaveChangesAsync();
                return null;
            }
        }
        public async Task RemoveMission(Mission mission)
        {
            var List = _context.Missions.Where(a => a.AgentId == mission.AgentId && a.TargetId == mission.TargetId);
            // ריצה בלולאה על כל הערכים בכדי למחוק
            foreach (var item in List)
            {
                // תנאי מיוחד בכדי שלא ימחוק לי את המשימה הנוכחית
                if (item != mission)
                {
                    _context.Remove(item);
                }
            }
            await _context.SaveChangesAsync();

        }


        public async Task UpdateMissions()
        {
            // שליפה והכנסה לתוך ליסט כולל המזהה של סוכן ומטרה
            var missions = await _context.Missions.Include(m => m.AgentId).Include(m => m.TargetId).ToListAsync();
            // ריצה על כל המשימות
            foreach (var mission in missions)
            {
                if (mission.Status == MissionStatus.Status.InMission.ToString())
                {
                    continue;
                }
                var agent = mission.AgentId;
                var target = mission.TargetId;

                string move = OrederMove(agent, target);
                LocationService.VerifyingLocation(agent.Location, move);


                if (agent.Location.X == target.Location.X && agent.Location.Y == target.Location.Y)
                {
                    target.Status = TargetStatus.Status.Dead.ToString();
                    agent.Status = AgentStatus.Status.NotActiv.ToString();
                    mission.TimeLeft = 0;
                    mission.ExecutionTime += 0.2;
                    mission.Status = MissionStatus.Status.Finish.ToString();
                    await _context.SaveChangesAsync();
                    continue;
                }


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

        public static string OrederMove(Agent agent, Target target)
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