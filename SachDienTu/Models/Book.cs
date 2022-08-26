//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SachDienTu.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Book()
        {
            this.AuthorContributes = new HashSet<AuthorContribute>();
            this.ImageDistributions = new HashSet<ImageDistribution>();
            this.InvoiceDetails = new HashSet<InvoiceDetail>();
        }
    
        public long ID { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public double price { get; set; }
        public long categoryId { get; set; }
        public long publishingHouseId { get; set; }
        public string description { get; set; }
        public int stateId { get; set; }
        public long views { get; set; }
        public string pdf { get; set; }
        public System.DateTime createAt { get; set; }
        public Nullable<System.DateTime> updateAt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AuthorContribute> AuthorContributes { get; set; }
        public virtual Category Category { get; set; }
        public virtual PublishingHouse PublishingHouse { get; set; }
        public virtual BookState BookState { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageDistribution> ImageDistributions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}