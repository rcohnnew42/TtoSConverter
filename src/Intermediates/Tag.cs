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
    
    public partial class Tag
    {
        public Tag()
        {
            this.CustomerTags = new HashSet<CustomerTag>();
        }
    
        public string TagId { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<CustomerTag> CustomerTags { get; set; }
    }
}
