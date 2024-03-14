using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.Models
{
    public class OrderHeader
    {
        // Unik identifierare för orderhuvudet
        public int Id { get; set; }

        // Id för användaren som skapat ordern
        public string ApplicationUserId { get; set; }

        // Anger att ApplicationUserId-egenskapen är en främmande nyckel för ApplicationUser-entiteten
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        // Användaren som skapat ordern
        public ApplicationUser ApplicationUser { get; set; }


        // Datum då ordern skapades
        public DateTime OrderDate { get; set; }
        // Datum då ordern skickades
        public DateTime ShippingDate { get; set; }


        // Den totala kostnaden för ordern
        public double OrderTotal { get; set; }


        // Status för ordern
        public string? OrderStatus { get; set; }
        // Status för betalningen av ordern (t.ex. betald, obetald)
        public string? PaymentStatus { get; set; }
        // Spårningsnummer för ordern
        public string? TrackingNumber { get; set; }
        // Transportör för ordern (t.ex. PostNord, DHL)
        public string? Carrier { get; set; }

        // Datum då betalningen genomfördes
        public DateTime PaymentDate { get; set; }





        //Stripe-betalningar

        // SessionId för Stripe-betalningen
        public string? SessionId {  get; set; }
        // PaymentIntentId för Stripe-betalningen
        public string? PaymentIntentId { get; set; }


        //Info om kunden. Namn, telefonnummer, adress.
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }
    }
}