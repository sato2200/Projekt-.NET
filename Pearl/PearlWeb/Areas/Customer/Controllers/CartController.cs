using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pearl.DataAccess.Data.Repository;
using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using Pearl.Models.ViewModels;
using Pearl.Utility;
using Stripe.Checkout;
using System.Security.Claims;


namespace PearlWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        // En bindningsegenskap som innehåller data för en varukorgsvy-modell
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }


        // Konstruktorn för CartController som tar emot ett enhetsverk som en beroende injektion
        public CartController(IUnitOfWork unitOfWork)
        {
            // Tilldelar den injicerade enhetsverksreferensen till det privata fältet
            _unitOfWork = unitOfWork;
        }

       

        public IActionResult Index()
        {
            // Hämtar användar-ID från autentiseringsuppgifter
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Skapar en ShoppingCartVM-instans för att hålla varukorgsdata och beställningsinformation
            ShoppingCartVM = new()
            {
                // Hämtar alla varor i varukorgen för den aktuella användaren, inklusive associerade produkter
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                // Skapar en ny OrderHeader för att räkna ut den totala beställningskostnaden
                OrderHeader = new()
            };


            // Itererar genom varje artikel i varukorgen för att beräkna den totala beställningskostnaden
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                // Hämtar priset per enhet för varje produkt i varukorgen
                double pricePerItemn = cart.Product.ListPrice;
                // Lägger till priset per enhet multiplicerat med antalet enheter till OrderTotal i ShoppingCartVM
                ShoppingCartVM.OrderHeader.OrderTotal += (pricePerItemn * cart.Count);

            }
            // Returnerar vyn med ShoppingCartVM som innehåller varukorgsdata och beställningsinformation
            return View(ShoppingCartVM);
        }





        public IActionResult Summary()
        {
            // Hämtar användar-ID från autentiseringsuppgifter
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Skapar en ShoppingCartVM-instans för att hålla varukorgsdata och beställningsinformation
            ShoppingCartVM = new()
            {
                // Hämtar alla varor i varukorgen för den aktuella användaren, inklusive associerade produkter
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                // Skapar en ny OrderHeader för att hålla användarinformation för beställning
                OrderHeader = new()
            };


            // Hämtar användarinformation från databasen och tilldelar den till OrderHeader i ShoppingCartVM
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            // Itererar genom varje artikel i varukorgen för att beräkna den totala beställningskostnaden
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                // Tilldelar priset per enhet från varukorgen till Price-egenskapen
                cart.Price = cart.Product.ListPrice;
                // Lägger till priset per enhet multiplicerat med antalet enheter till OrderTotal i OrderHeader i ShoppingCartVM
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }
            // Returnerar vyn med ShoppingCartVM som innehåller varukorgsdata och användarinformation för beställning
            return View(ShoppingCartVM);
        }





        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            // Hämtar användar-ID från autentiseringsuppgifter
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Hämtar alla varor i varukorgen för den aktuella användaren, inklusive associerade produkter
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");


            // Tilldelar aktuellt datum till OrderDate och användar-ID till ApplicationUserId i OrderHeader
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            // Hämtar användarinformation från databasen och tilldelar den till en ApplicationUser-variabel
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);



            // Beräknar den totala beställningskostnaden genom att iterera genom varje artikel i varukorgen
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                // Tilldelar priset per enhet från varukorgen till Price-egenskapen
                cart.Price = cart.Product.ListPrice; 
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }


            // Tilldelar betalningsstatus och beställningsstatus till OrderHeader
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

            // Skapar en ny OrderHeader och sparar den i databasen
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            // Skapar orderdetaljer för varje artikel i varukorgen och sparar dem i databasen
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }


            // Skapar betalningssession med Stripe för att behandla betalningen

            var domain = "https://localhost:7190/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // Konverterar priset till ören
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            // Skapar en betalningssession med Stripe och uppdaterar OrderHeader med betalnings-ID
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();


            // Redirectar användaren till Stripe för att slutföra betalningen
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }



        // Bekräfta en beställning och hantera betalningsstatus
        public IActionResult OrderConfirmation(int id)
        {
            // Hämtar OrderHeader från databasen inklusive ApplicationUser
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
           

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            // Kontrollerar om betalningen är slutförd
            if (session.PaymentStatus.ToLower() == "paid")
                {
                // Uppdaterar OrderHeader med Stripe-betalnings-ID och status för godkänd betalning
                _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
                }
            //}

            // Hämtar alla objekt i varukorgen för användaren och tar bort dem från databasen
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            // Returnerar vyn med id för den bekräftade beställningen
            return View(id);
        }



        // Metod för att uppdatera det totala antalet varor i kundkorgen för en användare
        private void UpdateTotalItemCount(string userId)
        {
            // Hämta alla varor i kundkorgen för den aktuella användaren
            var userCartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId);

            // Beräkna det totala antalet varor i kundkorgen
            int totalItemsInCart = userCartItems.Sum(item => item.Count);

            // Spara det totala antalet varor i sessionen
            HttpContext.Session.SetInt32(SD.SessionCart, totalItemsInCart);
        }




        // Åtgärd för att öka antalet av en produkt i kundvagnen med 1
        public IActionResult Plus(int cartId)
        {
            // Hämtar kundvagnsposten från databasen baserat på det givna cartId
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

            // Ökar antalet av produkten i kundvagnen med ett enhetsvärde
            cartFromDb.Count += 1;

            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();

            // Uppdaterar det totala antalet varor i kundkorgen för den aktuella användaren
            UpdateTotalItemCount(cartFromDb.ApplicationUserId);

            // Omdirigerar tillbaka till kundvagnssidan efter ökningen
            return RedirectToAction(nameof(Index));
        }


        // Åtgärd för att minska antalet av en produkt i kundvagnen med 1
        public IActionResult Minus(int cartId)
        {
            // Hämtar kundvagnsposten från databasen baserat på det givna cartId, spårar förändringar
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);
            if (cartFromDb.Count <= 1)
            {
                // Om antalet av produkten är mindre än eller lika med 1, tas den bort från kundvagnen
                // Uppdaterar det totala antalet varor i kundkorgen för den aktuella användaren
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
              .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                // Minskar antalet av produkten med ett enhetsvärde
                cartFromDb.Count -= 1;

                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();

            // Uppdaterar det totala antalet varor i kundkorgen för den aktuella användaren
            UpdateTotalItemCount(cartFromDb.ApplicationUserId);

            // Omdirigerar tillbaka till kundvagnssidan efter minskningen
            return RedirectToAction(nameof(Index));
        }




        // Ta bort en produkt från kundvagnen
        public IActionResult Remove(int cartId)
        {
            // Hämtar kundvagnsposten från databasen baserat på det givna cartId
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);

            // Uppdaterar det totala antalet varor i kundkorgen för den aktuella användaren
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
               .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);


            // Tar bort kundvagnsposten från databasen
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();


            // Uppdaterar det totala antalet varor i kundkorgen för den aktuella användaren
            UpdateTotalItemCount(cartFromDb.ApplicationUserId);


            // Omdirigerar tillbaka till kundvagnssidan efter borttagningen
            return RedirectToAction(nameof(Index));
        }
    }
}