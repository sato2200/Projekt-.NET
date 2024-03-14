using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using Pearl.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace PearlWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // Hämta användarens identitet från autentiseringsuppgifterna
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Hämtar användarens id
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            // Kontrollera om användaren är autentiserad
            if (claim != null)
            {
                // Hämta alla varor i kundkorgen för den aktuella användaren
                var userCartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value);

                // Beräkna det totala antalet varor i kundkorgen
                int totalItemsInCart = userCartItems.Sum(item => item.Count);

                // Spara det totala antalet varor i sessionen
                HttpContext.Session.SetInt32(SD.SessionCart, totalItemsInCart);
            }


            // Hämta alla produkter inklusive relaterade kategorier för att visa på startsidan
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            // Returnera vyn med produktlistan för att visas på startsidan
            return View(productList);
        }


        //Detaljvy för en produkt
        public IActionResult Details(int productId)
        {
            // Skapa ett nytt shoppingcart-objekt med den valda produkten och en räknare på 1
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            // Returnera vyn med detaljerna för produkten och objektet för varukorgen
            return View(cart);
        }




        // Lägga till eller uppdatera en produkt i varukorgen (POST-åtgärd)
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // Hämta användarens id från inloggad identitet
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Tilldela användar-ID till den nya eller uppdaterade varukorgen
            shoppingCart.ApplicationUserId = userId;

            // Kontrollera om en liknande produkt redan finns i varukorgen för samma användare
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            // Om varukorgen inte är tom
            if (cartFromDb != null)
            {
                // Varan finns redan i varukorgen, uppdatera antalet
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                // Om varukorgen är tom, lägg till den nya varan
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            _unitOfWork.Save();

            // Hämta alla varor i kundkorgen för den aktuella användaren
            var userCartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId);

            // Beräkna det totala antalet varor i kundkorgen
            int totalItemsInCart = userCartItems.Sum(item => item.Count);

            // Spara det totala antalet varor i sessionen
            HttpContext.Session.SetInt32(SD.SessionCart, totalItemsInCart);

            TempData["success"] = "Cart updated successfully";

            // Omdirigera till indexsidan för produkter
            return RedirectToAction(nameof(Index));
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


    }
}