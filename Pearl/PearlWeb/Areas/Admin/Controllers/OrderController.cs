using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using Pearl.Models.ViewModels;
using Pearl.Utility;
using Stripe;
using System.Security.Claims;

namespace PearlWeb.Areas.Admin.Controllers
{
    // Anger att kontrollern tillhör Admin-området
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;


        [BindProperty]
        public OrderVM OrderVM { get; set; }



        // Konstruktor för OrderController
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        // Visa startsidan för Order-hanteringen
        public IActionResult Index()
        {
            return View();
        }



        // Visa detaljer för en specifik order
        public IActionResult Details(int orderId)
        {
            // Hämta order och tillhörande detaljer
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }



        // Uppdatera orderdetaljer
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            // Uppdatera orderdetaljer med information från OrderVM
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;


            // Kontrollera om bärare och spårningsnummer är tillhandahållna i OrderVM
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }

            // Kontrollerar om det finns något värde för spårningsnumret i OrderVM och
            // tilldelar det värdet till det befintliga orderhuvudets spårningsnummer om det finns.
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }



        // Börja behandla en order
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult StartProcessing()
        {
            // Uppdaterar status för den valda orderhuvuden till "Pågående" i databasen.
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";

            // Omdirigerar användaren till sidan för orderdetaljer för den aktuella ordern.
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }



        //Skicka en order
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult ShipOrder()
        {
            // Hämtar orderhuvudet från databasen med hjälp av det medskickade orderns id.
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            // Tilldelar orderhuvudet spårningsnummer och fraktföretag från de medskickade orderuppgifterna.
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;

            // Uppdaterar status för den valda ordern till "Skickad" i databasen.
            orderHeader.OrderStatus = SD.StatusShipped;

            // Tilldelar frakt-datumet för ordern till dagens datum och tid.
            orderHeader.ShippingDate = DateTime.Now;

            // Uppdaterar orderhuvudet i databasen med de nya uppgifterna.
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";

            // Omdirigerar användaren till sidan för orderdetaljer för den aktuella ordern.
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }




        //Avboka en order
        [HttpPost]
        // Kräver att användaren har administratörsrollen för att kunna använda denna åtgärd.
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelOrder()
        {
            // Hämtar orderhuvudet från databasen med hjälp av det medskickade orderns id.
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            // Kontrollerar om betalningsstatus för ordern är "Godkänd".
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                // Skapar en återbetalningsförfrågan för den aktuella ordern.
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                // Skapar en återbetalningstjänst för att utföra återbetalningen.
                var service = new RefundService();
                Refund refund = service.Create(options);

                // Uppdaterar status för den aktuella ordern till "Avbruten" och betalningsstatus till "Återbetald" i databasen.
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                // Om betalningsstatus inte är "Godkänd", uppdatera endast status för ordern till "Avbruten" i databasen.
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";

            // Omdirigerar användaren till sidan för orderdetaljer för den aktuella ordern.
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

        }




        #region API CALLS


        //Hämta alla ordrar
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            // Skapa en tom lista för att lagra orderhuvuden
            IEnumerable<OrderHeader> objOrderHeaders;

            // Kontrollera användarroll för att avgöra vilka order som ska hämtas
            if (User.IsInRole(SD.Role_Admin))
            {
                // Hämta alla orderhuvuden för administratörer inklusive användaruppgifter
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                // Hämta användaridentitet och hämta användar-ID
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Hämta alla orderhuvuden för den inloggade användaren inklusive användaruppgifter
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }



            // Filtrera orderhuvuden baserat på status
            switch (status)
            {
                //case "pending":
                //    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                //    break;

                // Visa bara order i process
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                // Visa bara avslutade order
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;

                // Visa bara godkända order
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }

            // Returnera JSON-data som innehåller de filtrerade orderhuvudena
            return Json(new { data = objOrderHeaders });

            #endregion
        }
    }
}

