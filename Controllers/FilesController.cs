using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebDataTransfer.Logics;

namespace WebDataTransfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ExcelFloder _floder;

        public FilesController(ExcelFloder floder)
        {
            _floder = floder;
        }
        [HttpPost]
        public async Task<IActionResult> PostData(IFormFile file)
        {
            await _floder.ExcelData(file);
            return Ok("File processed");
        }
    }
}
