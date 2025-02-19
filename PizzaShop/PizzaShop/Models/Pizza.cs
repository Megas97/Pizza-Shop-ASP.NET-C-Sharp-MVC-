//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PizzaShop.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pizza
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pizza()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int PizzaID { get; set; }
        public string PizzaName { get; set; }
        public int RecipeID { get; set; }
        public double PizzaPrice { get; set; }
        public string PizzaPicturePath { get; set; }
        public string PizzaSize { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
