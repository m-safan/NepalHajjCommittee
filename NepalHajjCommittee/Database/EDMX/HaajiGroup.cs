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
    
    public partial class HaajiGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HaajiGroup()
        {
            this.Batches = new HashSet<Batch>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public System.DateTime ArrivalDateMakkah { get; set; }
        public System.DateTime DepartureDateMakkah { get; set; }
        public System.DateTime ArrivalDateMadinah { get; set; }
        public System.DateTime DepartureDateMadinah { get; set; }
        public string IncomingFlight { get; set; }
        public string OutgoingFlight { get; set; }
        public string StateName { get; set; }
        public int VisitYear { get; set; }
        public bool IsRoomAllotedMakkah { get; set; }
        public bool IsRoomAllotedMadinah { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Batch> Batches { get; set; }
    }
}
