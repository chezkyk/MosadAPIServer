using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Statuses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MosadAPIServer.Services
{
    public class AgentService
    {
        private readonly MosadDbContext _context;

        // ביצוע הזרקה ע''פ עקרון DI
        public AgentService(MosadDbContext context)
        {
            _context = context;
        }

        // פונקציה זאת יוצרת סוכן ומוסיפה אותו ל DB
        public async Task<Agent> CreateAgent(Agent agent)
        {
            // עדכון סטטוס סוכן שנוצר ללא פעיל
            agent.Status = AgentStatus.Status.NotActiv.ToString();
            _context.Agents.Add(agent);// הוספת הסוכן ל DB
            await _context.SaveChangesAsync();// שמירת השינויים
            return agent;
        }

        //-- פונקציה זאת מחזירה את כל הסוכנים מה DB
        public async Task<List<Agent>> GetAllAgents()
        {
            // החזרת כל הסוכנים שנמצאים ב DB
            return await _context.Agents.ToListAsync();
        }

        // פונקציה זאת מעדכת מיקום של סוכן
        public async Task<Agent> UpdateLocation(int id, Location location)
        {
            // למשתנה agent ייכנס הסוכן הספציפי עם המזהה שהגיע בחתימת הפונקציה
            var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == id);
            if (agent.Location == null)// בדיקה זאת נצרכת לפעם הראשונה שאז המיקום תמיד שווה NULL
            {
                agent.Location = new Location();
            }
            agent.Location.X = location.X;//  עדכון קאורדינטת X
            agent.Location.Y = location.Y;// עדכון קאורדינטת Y

            await CreateMissions(agent);
            _context.Update(agent);// הוספת השינויים ל DB
            await _context.SaveChangesAsync();// שמירת השינויים
            return agent;
        }
        // פונקציה זאת מעדכנת כיוון 
        public async Task<Agent> UpdateDirection(int id, string direction)
        {
            // למשתנה agent ייכנס הסוכן הספציפי עם המזהה שהגיע בחתימת הפונקציה
            var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == id);
            //בדיקת ולידטציה של מיקום
            LocationService.VerifyingLocation(agent.Location, direction);
            _context.Update(agent);// הוספת השינויים ל DB
            await _context.SaveChangesAsync();// שמירת השינויים
            return agent;
        }
        public async Task CreateMissions(Agent agent)
        {
            var list = await _context.Targets.ToArrayAsync();
                                                               
            foreach (Target target in list)
            {
                if (target.Status == TargetStatus.Status.Alive.ToString()) 
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
		public async Task<bool> IfNotTarget(int? id)
		{
			var mission = await _context.Missions.FirstOrDefaultAsync(x => x.TargetId.Id == id);
			return mission == null || mission.Status == MissionStatus.Status.Offer.ToString();

		}










	}
}
