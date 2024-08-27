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

            _context.Update(agent);// הוספת השינויים ל DB
            await _context.SaveChangesAsync();// שמירת השינויים
            return agent;
        }


        // פונקציה זאת מעדכנת כיוון 
        public async Task<Agent> UpdateDirection(Agent agent, string direction)
        {
            
            //בדיקת ולידטציה של מיקום
            LocationService.VerifyingLocation(agent.Location, direction);
            _context.Update(agent);// הוספת השינויים ל DB
            await _context.SaveChangesAsync();// שמירת השינויים
            return agent;
        }


        // פונקציה זאת בודקת אפשרות ציוות ומצוות סוכן ומטרה
        public async Task FindMissions(Agent agent)
        {
            // הכנסת כל המטרות לרשימה
            var list = await _context.Targets.ToArrayAsync();
             // ריצה על הרשימה                                                  
            foreach (Target target in list)
            {
                // בדיקה שמטרה עדיין לא חוסלה 
                if (target.Status == TargetStatus.Status.Alive.ToString()) 
                {
                    // בדיקה אם המרחק מספיק קרוב
                    if (MissionService.IfMission(agent, target)) 
                    {
                        // יצירת המשימה לאחר כל האימותים
                        Mission mission = MissionService.CreateMission(agent, target); //יצירת משימה
                        _context.Missions.Add(mission); // הוספה ל DB
                        _context.SaveChanges();
                    }
                }
            }
            return;
        }


		public async Task<bool> IfNotTarget(int? id)
		{
            // שולף ךתוך מערך את כל המשימות שהמטרה נמצאת שם
			var missions = await _context.Missions.Where(x => x.TargetId.Id == id).ToListAsync();
            foreach (var item in missions)//ריצה על כל איברי המערך
            {
                // בדיקה עם הוא נמצא כבר במשימה שנמצאת כרגע בפעולה
                if (item.Status == MissionStatus.Status.InMission.ToString())
                {
                    return false;
                }
            }
            return true;

		}










	}
}
