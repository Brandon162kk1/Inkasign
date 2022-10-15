using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Inkasign.Controllers
{
    public class NosotrosController : Controller
    {
    private readonly ILogger<NosotrosController> _logger;

    public NosotrosController(ILogger<NosotrosController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    }
}