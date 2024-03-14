using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pearl.DataAccess.Data;
using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using Pearl.Utility;

namespace PearlWeb.Areas.Admin.Controllers
{
    // Anger att kontrollern tillhör Admin-området
    [Area("Admin")]
    // Anger att användaren måste ha rollen Admin för att få tillgång till denna controller
    [Authorize(Roles=SD.Role_Admin)]
	public class CategoryController : Controller
    {

        // Enhet för arbetsenhet (UnitOfWork) för att hantera databasoperationer
        private readonly IUnitOfWork _unitOfWork;


        public CategoryController(IUnitOfWork unitOfWork)
        {
            // Injicera arbetsenhet i kontrollerkonstruktorn
            _unitOfWork = unitOfWork;
        }



        public IActionResult Index()
        {
            // Hämta en lista över alla kategorier från arbetsenheten och konvertera till en lista
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();

            // Returnera vyn med listan över kategorier som modelldata
            return View(objCategoryList);
        }



        public IActionResult Create()
        {
            return View();
        }



        //När en ny kategori skapas i formulär
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The displayOrder cannot match the Name");
            }
            // Kontrollera om modellens egenskaper uppfyller valideringsreglerna
            if (ModelState.IsValid)
            {
                // Lägg till den nya kategorin i arbetsenheten
                _unitOfWork.Category.Add(obj);
                // Spara ändringar i databasen
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";

                // Omdirigera till Index-åtgärden för att visa listan över kategorier
                return RedirectToAction("Index");
            }
            // Om modellen inte är giltig, visa skaparvyn igen med valideringsfel
            return View();
        }



        // Redigera en befintlig kategori baserat på dess id
        public IActionResult Edit(int? id) 
        {
            //Om id är tomt eller noll
            if (id == null || id == 0) 
            {
                // Returnera NotFound-resultat om id är ogiltligt
                return NotFound();
            }

            // Hämta kategorin från arbetsenheten baserat på id
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);


            // Kontrollera om kategorin inte hittades
            if (categoryFromDb == null)
            {
                // Returnera NotFound-resultat om kategorin inte hittades
                return NotFound();
            }

            // Returnera vyn för att redigera kategorin med den befintliga kategorin som modelldata
            return View(categoryFromDb);
        }



        // Hantera postningsdata när en befintlig kategori redigeras
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            // Kontrollera om modellens egenskaper uppfyller valideringsreglerna
            if (ModelState.IsValid)
            {
                // Uppdatera den befintliga kategorin i arbetsenheten
                _unitOfWork.Category.Update(obj);
                // Spara ändringar i databasen
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";

                // Omdirigera till Index-åtgärden för att visa listan över kategorier
                return RedirectToAction("Index");
            }

            // Om modellen inte är giltig, visa redigeringsvyn igen med valideringsfel
            return View();
        }



        // Borttagning av en befintlig kategori baserat på dess id
        public IActionResult Delete(int? id)
        {
            // Kontrollera om id är tomt eller 0
            if (id == null || id == 0)
            {
                // Returnera NotFound-resultat om id är ogiltigt
                return NotFound();
            }
            // Hämta kategorin från arbetsenheten baserat på id
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);


            // Kontrollera om kategorin inte hittades
            if (categoryFromDb == null)
            {
                // Returnera NotFound-resultat om kategorin inte hittades
                return NotFound();
            }
            // Returnera vyn för att bekräfta borttagning av kategorin med den befintliga kategorin som modelldata
            return View(categoryFromDb);
        }



        // Hantera postningsdata när en befintlig kategori raderas
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            // Hämta kategorin från arbetsenheten baserat på id
            Category obj = _unitOfWork.Category.Get(u => u.Id == id);

            // Kontrollera om kategorin inte hittades
            if (obj == null)
            {
                // Returnera NotFound-resultat om kategorin inte hittades
                return NotFound();
            }

            // Ta bort den befintliga kategorin från arbetsenheten
            _unitOfWork.Category.Remove(obj);
            // Spara ändringar i databasen
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";

            // Omdirigera till Index-åtgärden för att visa listan över kategorier
            return RedirectToAction("Index");
        }
    }
}
