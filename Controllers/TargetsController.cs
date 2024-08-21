using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MosadAPIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly TargetsController _context;

        public TargetsController(TargetsController context)
        {
            this._context = context;
        }
    }
}
