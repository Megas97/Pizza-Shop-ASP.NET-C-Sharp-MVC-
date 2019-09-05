using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Models
{
    [MetadataType(typeof(IngredientMetadata))]
    public partial class Ingredient
    {

    }

    public class IngredientMetadata
    {
        [Display(Name = "Ingredient ID")]
        public int IngredientID { get; set; }

        [Display(Name = "Ingredient Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingredient name is required")]
        public string IngredientName { get; set; }

        [Display(Name = "Ingredient Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingredient price is required")]
        public double IngredientPrice { get; set; }
    }
}