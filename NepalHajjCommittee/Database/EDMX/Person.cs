//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NepalHajjCommittee.Database.EDMX
{
    using System;
    using System.Collections.Generic;
    
    public partial class Person
    {
        public int ID { get; set; }
        public int FK_ID_Batch { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string PassportNo { get; set; }
        public Nullable<int> FK_ID_Bed_Makkah { get; set; }
        public Nullable<int> FK_ID_Bed_Madinah { get; set; }
        public string BusNumber { get; set; }
        public string Photo { get; set; }
        public string ContactNo { get; set; }
    
        public virtual Batch Batch { get; set; }
        public virtual Bed Bed { get; set; }
        public virtual Bed Bed1 { get; set; }
    }
}