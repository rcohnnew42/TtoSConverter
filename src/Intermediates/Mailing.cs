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
    
    public partial class Mailing
    {
        public Mailing()
        {
            this.MailingsCustomers = new HashSet<MailingsCustomer>();
        }
    
        public string MailingId { get; set; }
        public string Name { get; set; }
        public System.DateTime SentAt { get; set; }
    
        public virtual ICollection<MailingsCustomer> MailingsCustomers { get; set; }
    }
}