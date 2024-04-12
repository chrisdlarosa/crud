using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CountriesController
        public ActionResult Index()
        {
            var Datos = _context.countries.Include(c=>c.Regions).ToList();
            return View(Datos);
        }


        // GET: CountriesController/Create
        public async Task<IActionResult> Create(string? id, string? modo) 
        {
            string? vModo = modo; 
            Countries? Reg = new Countries();
            
            if (id == "" || id == "0")
            {
                return View("Index", Reg);

            }
            else
            {
                Reg = await _context.countries.FindAsync(id);
                Countries countrymodel = new Countries();
                countrymodel.COUNTRY_NAME = Reg.COUNTRY_NAME;
                countrymodel.COUNTRY_ID = Reg.COUNTRY_ID;
                countrymodel.REGION_ID = Reg.REGION_ID;
                return View("Index", countrymodel);
            }



        }

        // POST: CountriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Countries country, string id)
        {

            if (ModelState.IsValid)
            {
                if (id == "add")
                {

                    await _context.countries.AddAsync(country);
                    await _context.SaveChangesAsync();

                    TempData["mensaje"] = "El Pais se guardo correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.countries.Update(country);
                    await _context.SaveChangesAsync();

                    TempData["mensaje"] = "Cambios guardados correctamente";
                    return RedirectToAction(nameof(Index), new { id = "" });
                }
            }

            return View(country);
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            var reg = _context.countries.Find(id);
            if (reg == null)
            {
                return Json(new { success = false, message = "Algo salió mal... inténtalo de nuevo." });
            }
            else
            {

                _context.countries.Remove(reg);
                _context.SaveChanges();
                return Json(new { success = true, message = "Pais eliminada exitosamente." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatos()
        {
            var todos = await _context.countries.Include(c => c.Regions).ToListAsync();
            return Json(new { data = todos });
        }

    }
}
