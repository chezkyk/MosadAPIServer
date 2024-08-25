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
    [Route("api/[controller]")]
    [ApiController]
    public class MissionsController : ControllerBase
    {
        private readonly MissionService _missionService;

        public MissionsController(MissionService missionService)
        {
            _missionService = missionService;
        }

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
            // הכנסת LIST של סוכנים לתוך המשתנה agent
            var missions = await _missionService.GetAllMissions();
            return Ok(new { missions = missions });
        }
        //-- Update status
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            await _missionService.UpdateMissionStatus(id);
            return Ok();

        }
        

    }
}










