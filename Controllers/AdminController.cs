using Microsoft.AspNetCore.Mvc;
using Inkasign.Data;
using Microsoft.AspNetCore.Authorization;

namespace Inkasign.Controllers
{
    
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context){
            _context = context;
        }

        public IActionResult Panel(){
            return View();
        }

    }
}