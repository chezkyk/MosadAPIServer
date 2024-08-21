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
    }
}
