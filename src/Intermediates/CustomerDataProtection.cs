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
    
    public partial class CustomerDataProtection
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string StatementId { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual DataProtectionStatement DataProtectionStatement { get; set; }
    }
}
