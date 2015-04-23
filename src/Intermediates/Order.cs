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
    
    public partial class Order
    {
        public Order()
        {
            this.Donations = new HashSet<Donation>();
            this.MembershipSubscriptions = new HashSet<MembershipSubscription>();
            this.Merchandises = new HashSet<Merchandise>();
            this.OrderAttributes = new HashSet<OrderAttribute>();
            this.Tickets = new HashSet<Ticket>();
        }
    
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public System.DateTime DateTime { get; set; }
        public string SalesChannel { get; set; }
        public string UserName { get; set; }
        public string Notes { get; set; }
        public string Answer { get; set; }
        public Nullable<decimal> OrderFee { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual ICollection<Donation> Donations { get; set; }
        public virtual ICollection<MembershipSubscription> MembershipSubscriptions { get; set; }
        public virtual ICollection<Merchandise> Merchandises { get; set; }
        public virtual ICollection<OrderAttribute> OrderAttributes { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}