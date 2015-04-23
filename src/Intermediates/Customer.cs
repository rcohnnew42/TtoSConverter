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
    
    public partial class Customer
    {
        public Customer()
        {
            this.CustomerAttributes = new HashSet<CustomerAttribute>();
            this.Orders = new HashSet<Order>();
            this.Relationships = new HashSet<Relationship>();
            this.Relationships1 = new HashSet<Relationship>();
            this.CustomerDataProtections = new HashSet<CustomerDataProtection>();
            this.CustomerTags = new HashSet<CustomerTag>();
            this.MailingsCustomers = new HashSet<MailingsCustomer>();
        }
    
        public string CustomerId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganisationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string PostTown { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
    
        public virtual ICollection<CustomerAttribute> CustomerAttributes { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
        public virtual ICollection<Relationship> Relationships1 { get; set; }
        public virtual ICollection<CustomerDataProtection> CustomerDataProtections { get; set; }
        public virtual ICollection<CustomerTag> CustomerTags { get; set; }
        public virtual ICollection<MailingsCustomer> MailingsCustomers { get; set; }
    }
}