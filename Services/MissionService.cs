using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;

namespace MosadAPIServer.Services
{
    public class MissionService
    {
        public static double CalculateTimeLeft(Agent agent, Target target)
        {
            double distance = CalculateDistance(agent.Location.X,agent.Location.Y,target.Location.X,target.Location.Y);
            double timeLeft = distance / 5;
            return timeLeft;

        }
        public static double CalculateDistance(double xa, double ya, double xt, double yt)
        {
            return Math.Sqrt(Math.Pow(xt - xa, 2) + Math.Pow(yt - ya, 2));
        }
        public static void UpdateMissionTime(Mission mission)
        {
            mission.ExecutionTime += 0.2;

        }

        public static Mission CreateMission(Agent agent, Target target)
        {
            Mission mission = new Mission();
            mission.AgentId = agent;
            mission.TargetId = target;
            mission.ExecutionTime = 0;
            mission.TimeLeft = CalculateTimeLeft(agent, target);
            mission.Status = MissionStatus.Status.Offer.ToString();
            return (mission);
        }

        public static bool IfMission(Agent agent, Target target)
        {
            if (agent == null || target == null || agent.Location == null || target.Location == null)
            {
                return false;
            }
            double distance = CalculateDistance(agent.Location.X, agent.Location.Y, target.Location.X, target.Location.Y);
            if (distance < 200)
            {
                return true;
            }
            return false;
        }
        
    }
}
