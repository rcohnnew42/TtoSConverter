//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intermediates
{
    using System;
    using System.Collections.Generic;
    
    public partial class StockItem
    {
        public StockItem()
        {
            this.Merchandises = new HashSet<Merchandise>();
            this.StockItemAttributes = new HashSet<StockItemAttribute>();
        }
    
        public string StockItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int NumberInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitCost { get; set; }
    
        public virtual ICollection<Merchandise> Merchandises { get; set; }
        public virtual ICollection<StockItemAttribute> StockItemAttributes { get; set; }
    }
}
