using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pearl.Models
{
    // Klassen Category representerar kategoridata i applikationen
    public class Category
    {
        // Anger att Id-egenskapen är nyckeln för entiteten
        [Key]
        // Unik identifierare för kategorin
        public int Id { get; set; }


        // Anger att Name-egenskapen är obligatorisk och måste finnas med
        [Required]
        // Används för att ange ett mänskligt läsbart namn för egenskapen
        [DisplayName("Category Name")]
        // Namnet på kategorin
        public string Name { get; set; }

        // Anger att DisplayOrder-egenskapen är obligatorisk och måste finnas med
        [Required]
        // Används för att ange ett mänskligt läsbart namn för egenskapen
        [DisplayName("Display Order")]
        // Anger att värdet för DisplayOrder måste ligga mellan 1 och 1000
        [Range(1, 1000, ErrorMessage = "Display Order must be between 1-100")]


        // Ordningen för att visa kategorin, används för att sortera kategorier
        public int DisplayOrder { get; set; }
    }
}

