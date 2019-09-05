using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PizzaShop.Models
{
    [MetadataType(typeof(RecipeMetadata))]
    public partial class Recipe
    {

    }

    public class RecipeMetadata
    {
        [Display(Name = "Recipe ID")]
        public int RecipeID { get; set; }

        [Display(Name = "Recipe Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Recipe name is required")]
        public string RecipeName { get; set; }
    }
}