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
    
    public partial class StockItemAttribute
    {
        public string StockItemId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Id { get; set; }
    
        public virtual StockItem StockItem { get; set; }
    }
}