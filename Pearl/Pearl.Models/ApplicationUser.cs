using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.Models
{
    // ApplicationUser-klassen ärver från IdentityUser-klassen och representerar användaruppgifter i applikationen
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? StreetAddress { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? PostalCode { get; set; }
    }
}
