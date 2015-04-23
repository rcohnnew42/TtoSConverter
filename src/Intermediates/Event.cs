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
    
    public partial class Event
    {
        public Event()
        {
            this.EventAttributes = new HashSet<EventAttribute>();
            this.Instances = new HashSet<Instance>();
        }
    
        public string EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<short> Duration { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
    
        public virtual ICollection<EventAttribute> EventAttributes { get; set; }
        public virtual ICollection<Instance> Instances { get; set; }
    }
}
