using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.Utility
{
    // Innehåller olika statiska strängar för användning inom applikationen
    public static class SD
	{

        //Roller för användare
		public const string Role_Customer = "Customer";
		public const string Role_Admin = "Admin";

        //Orderstatus
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";


        //Betalningsstatus
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        //public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        // Nyckel för sessionsvariabel för kundvagn
        public const string SessionCart = "SessionShoppingCart";
    }
}
