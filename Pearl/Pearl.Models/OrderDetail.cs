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
    public class OrderDetail
    {
        // Unik identifierare för orderdetaljen
        public int Id { get; set; }

        // Anger att OrderHeaderId-egenskapen är obligatorisk och måste finnas med
        [Required]
        // Id för den överordnade orderhuvudet
        public int OrderHeaderId { get; set; }

        // Anger att OrderHeaderId-egenskapen är en främmande nyckel för OrderHeader-entiteten
        [ForeignKey("OrderHeaderId")]
        // Anger att OrderHeader-egenskapen inte ska valideras av modellvalideraren
        [ValidateNever]
        // Orderhuvudet kopplat till orderdetaljen
        public OrderHeader OrderHeader { get; set; }


        // Anger att ProductId-egenskapen är obligatorisk och måste finnas med
        [Required]
        // Id för produkten som är associerad med orderdetaljen
        public int ProductId { get; set; }



        // Anger att ProductId-egenskapen är en främmande nyckel för Product-entiteten
        [ForeignKey("ProductId")]
        // Anger att Product-egenskapen inte ska valideras av modellvalideraren
        [ValidateNever]
        // Produkten associerad med orderdetaljen
        public Product Product { get; set; }


        // Antal av produkten som ingår i orderdetaljen
        public int Count { get; set; }

        // Priset för varje produkt i orderdetaljen
        public double Price { get; set; }
    }
}
