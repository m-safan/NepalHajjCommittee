using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NepalHajjCommittee.Models
{
    public class SearchedResults
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string PassportNo { get; set; }
        public string ContactNo { get; set; }
        public string StateName { get; set; }
        public string GroupName { get; set; }
        public string BatchName { get; set; }
        public string MakkahRoomNo { get; set; }
        public string MadinahRoomNo { get; set; }
        public string MadinahToAirportBusNumber { get; set; }
        public string MakkahToAirportBusNumber { get; set; }
        public string MadinahToMakkahBusNumber { get; set; }
        public string MakkahToMadinahBusNumber { get; set; }
        public DateTime ArrivalDateMakkah { get; set; }
        public DateTime DepartureDateMakkah { get; set; }
        public DateTime ArrivalDateMadinah { get; set; }
        public DateTime DepartureDateMadinah { get; set; }
    }
}
