using BlueLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VillaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var villaList = _context.Villas.ToList();
            return View(villaList);
        }
    }
}
