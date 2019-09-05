using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Models
{
    [MetadataType(typeof(PizzaMetadata))]
    public partial class Pizza
    {

    }

    public class PizzaMetadata
    {
        [Display(Name = "Pizza ID")]
        public int PizzaID { get; set; }

        [Display(Name = "Pizza Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pizza name is required")]
        public string PizzaName { get; set; }

        [Display(Name = "Pizza Recipe")]
        public int RecipeID { get; set; }

        [Display(Name = "Pizza Price")]
        public double PizzaPrice { get; set; }

        [Display(Name = "Pizza Image")]
        public string PizzaPicturePath { get; set; }

        [Display(Name = "Pizza Size")]
        public string PizzaSize { get; set; }
    }
}