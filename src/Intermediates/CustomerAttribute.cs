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
    
    public partial class CustomerAttribute
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Id { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
