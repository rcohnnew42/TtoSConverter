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
    
    public partial class Instance
    {
        public Instance()
        {
            this.InstanceAttributes = new HashSet<InstanceAttribute>();
            this.LockedSeats = new HashSet<LockedSeat>();
            this.MaskedSeats = new HashSet<MaskedSeat>();
            this.Tickets = new HashSet<Ticket>();
        }
    
        public string InstanceId { get; set; }
        public string EventId { get; set; }
        public System.DateTime Start { get; set; }
        public string Notes { get; set; }
        public string SeatingPlanId { get; set; }
        public string PriceListId { get; set; }
        public string CommissionStructureId { get; set; }
        public string TicketDesignId { get; set; }
    
        public virtual Event Event { get; set; }
        public virtual ICollection<InstanceAttribute> InstanceAttributes { get; set; }
        public virtual PriceList PriceList { get; set; }
        public virtual ICollection<LockedSeat> LockedSeats { get; set; }
        public virtual ICollection<MaskedSeat> MaskedSeats { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
