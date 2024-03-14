using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pearl.DataAccess.Data;
using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using Pearl.Models.ViewModels;
using Pearl.Utility;
using System.Collections.Generic;

namespace PearlWeb.Areas.Admin.Controllers
{
    // Anger att kontrollern tillhör Admin-området och kräver administratörsbehörighet
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

        // Konstruktor för att injicera IUnitOfWork och IWebHostEnvironment
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}


        // Åtgärden för att visa alla produkter
        public IActionResult Index()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return View(objProductList);
		}


        // Åtgärden för att skapa eller uppdatera en produkt
        public IActionResult Upsert(int? id)
		{
            // Skapar ett nytt objekt av typen ProductVM för att hantera produktvy-modellen
            ProductVM productVM = new()
			{
                // Hämta en lista med kategorier för rullgardinsmenyn
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
                // Skapar ett nytt Product-objekt för att lagra produktinformationen
                Product = new Product()
			};


            // Kontrollerar om id är tomt eller noll, vilket indikerar att en ny produkt ska skapas
            if (id == null || id == 0)
			{
                // Returnerar vyn för att skapa en ny produkt
                return View(productVM);
			}
			else
			{
                // Hämtar befintlig produkt från databasen för att uppdatera den
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                // Returnerar vyn för att uppdatera produkten med den befintliga produktinformationen
                return View(productVM);
			}
			
		}


       
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            // Kontrollerar om modellens validering är korrekt
            if (ModelState.IsValid)
            {
                // Kontrollerar om en fil har bifogats för att ladda upp
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    // Om en ny bild har valts, ladda upp den som vanligt
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    // Kontrollerar om det finns en tidigare bild för produkten och tar bort den om så är fallet
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl) && productVM.Product.ImageUrl != @"\images\icon\no-image.jpg")
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Sparar den uppladdade filen till den angivna sökvägen
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    // Uppdaterar produktens bild-URL med den nya filens sökväg
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                else if (productVM.Product.Id == 0)
                {
                    // Om ingen ny bild har valts men det är en ny produkt, använd standardbilden
                    productVM.Product.ImageUrl = @"\images\icon\no-image.jpg";
                }

                // Kontrollerar om produkten är ny eller om den ska uppdateras
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    // Om ingen ny bild har valts och det inte är en ny produkt, behåll den befintliga bilden
                    if (file == null)
                    {
                        var existingProduct = _unitOfWork.Product.Get(p => p.Id == productVM.Product.Id);
                        if (existingProduct != null)
                        {
                            productVM.Product.ImageUrl = existingProduct.ImageUrl;
                        }
                    }
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                // Om valideringen misslyckas, fyller på CategoryList för att visa kategorirullgardinsmenyn
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                // Returnerar vyn med produktvy-modellen för att visa eventuella felmeddelanden och behålla användarens inskrivna data
                return View(productVM);
            }
        }

        #region APICALLS
        [HttpGet]
		public IActionResult GetAll()
		{
            // Hämtar en lista med alla produkter inklusive deras kategorier från databasen
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            // Returnerar en JSON-representation av produktlistan
            return Json(new { data = objProductList });
		}




		[HttpDelete]
		public IActionResult Delete(int? id)
		{
            // Hämtar produkten som ska raderas från databasen med det angivna id
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

            // Kontrollerar om produkten existerar
            if (productToBeDeleted == null)
			{
                // Returnerar ett felmeddelande om produkten inte kunde hittas
                return Json(new { success = false, message = "Error while deleting" });
			}


            // Hämtar den gamla bildsökvägen för produkten
            var oldImagePath =
						   Path.Combine(_webHostEnvironment.WebRootPath,
						   productToBeDeleted.ImageUrl.TrimStart('\\'));

            // Kontrollerar om den gamla bilden finns och tar bort den
            if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}
            // Tar bort produkten från databasen
            _unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();

            // Returnerar ett lyckat meddelande om att produkten raderades framgångsrikt
            return Json(new { success = true, message = "Delete Successful" });

		}
		#endregion
	}
}

