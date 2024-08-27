using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using MosadAPIServer.Services;
using MosadAPIServer.Statuses;
using System.Diagnostics.Metrics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MosadAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MissionsController : ControllerBase
    {
        private readonly MissionService _missionService;
        private readonly MosadDbContext _context;
        public MissionsController(MissionService missionService,MosadDbContext context)
        {
            _missionService = missionService;
            _context = context;
        }

        // 
        [HttpPost("update")]

        public async Task<IActionResult> Update()
        {
            await _missionService.UpdateMissions();
            return Ok();
        }


        //--Get All Missions
        [HttpGet]
        public async Task<IActionResult> GetAllMissions()
        {
            
            var missions = await _missionService.GetAllMissions();
            return Ok(missions);
        }


        //-- Update status
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var mission = await _missionService.UpdateMissionStatus(id);
            if (mission == null)
            {
                return BadRequest(400);
            }
            return Ok();

        }


        //returns how many offer missions is
        [HttpGet("SumOfOfferMissions")]
        public async Task<IActionResult> SumOfOfferMissions()
        {
            var offerMissionsCount = await  _context.Missions.Where(mission => mission.Status == MissionStatus.Status.Offer.ToString()).CountAsync();
            return Ok(offerMissionsCount);
        }


        //returns how many n mission missions is
        [HttpGet("SumOfInMissionMissions")]
        public async Task<IActionResult> SumOfInMissionMissions()
        {
            var inMissionCount = await _context.Missions.Where(mission => mission.Status == MissionStatus.Status.InMission.ToString()).CountAsync();
            return Ok(inMissionCount);
        }


        //returns how many finish missions is
        [HttpGet("SumOfFinishMissions")]
        public async Task<IActionResult> SumOfFinishMissions()
        {
            var finishMissionsCount = await _context.Missions.Where(mission => mission.Status == MissionStatus.Status.Finish.ToString()).CountAsync();
            return Ok(finishMissionsCount);
        }


        // פונקציה זאת מחזירה את כמות החיסולים לפי סוכן
        [HttpGet("AmountOfKills/{id}")]
        public async Task<IActionResult> AmountOfKills(int id)
        {
            var missionList = await _context.Missions.ToListAsync();
            int killsCount = 0;
            for (int i = 0; i < missionList.Count() ; i++)
            {
                if (missionList[i].Status == MissionStatus.Status.Finish.ToString() && missionList[i].AgentId.Id == id)
                {
                    killsCount++;
                }
            }
            return Ok(killsCount);  
        }


        // פונקציה זאת מחזירה כמה זמן נשאר עד לחיסול
		[HttpGet("TimeLeft/{id}")]
		public async Task<IActionResult> TimeLeft(int id)
		{
			var mission = await _context.Missions.FirstOrDefaultAsync(m => m.AgentId.Id == id);
            var timeLeft = mission.TimeLeft;
			return Ok(timeLeft);
		}


		//--Get One Missions
		[HttpGet("Get/{id}")]
		public async Task<IActionResult> GetMissionById(int id)
		{

			var mission = await _context.Missions.FirstOrDefaultAsync(m => m.AgentId.Id == id);
			return Ok(mission);
		}
	}
}










