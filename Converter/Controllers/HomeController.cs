using Converter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NReco.VideoConverter;

namespace Converter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult ConvertMp4ToAc3(IFormFile mp4File)
        {
            if (mp4File != null && mp4File.Length > 0)
            {
                if (Path.GetExtension(mp4File.FileName).ToLower() == ".mp4")
                {
                    string inputFilePath = Path.GetTempFileName();
                    string outputFilePath = Path.Combine(Path.GetTempPath(), "output.ac3");

                    using (var stream = new FileStream(inputFilePath, FileMode.Create))
                    {
                        mp4File.CopyTo(stream);
                    }

                    var ffMpeg = new FFMpegConverter();
                    ffMpeg.ConvertMedia(inputFilePath, outputFilePath, Format.ac3);

                    return File(System.IO.File.ReadAllBytes(outputFilePath), "audio/ac3", "output.ac3");
                }
                else
                {
                    ModelState.AddModelError("mp4File", "Please upload a valid MP4 file.");
                    return View();
                }
            }

            ModelState.AddModelError("mp4File", "Please select an MP4 file.");
            return View();
        }
    }
}