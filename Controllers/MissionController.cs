using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MosadAPIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly MissionController _context;

        public MissionController(MissionController context)
        {
            this._context = context;
        }
    }
}
