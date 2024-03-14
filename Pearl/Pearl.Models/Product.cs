using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.Models
{
	public class Product
	{
        [Key]
        public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Description { get; set; }


		[Required]
		[Display(Name = "List Price")]
		[Range(1, 10000)]
		public double ListPrice { get; set; }

		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
		[ValidateNever]
		public Category Category { get; set; }


		[ValidateNever]
        [Required]
        public string ImageUrl { get; set; }
	}
}