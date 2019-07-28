using NepalHajjCommittee.Database.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NepalHajjCommittee.Models
{
    public class RoomDTO : Room
    {
        public RoomDTO() { }

        public RoomDTO(Room room)
        {
            ID = room.ID;
            City = room.City;
            HotelName = room.HotelName;
            IsAvailable = room.IsAvailable;
            IsDirty = room.IsDirty;
            RoomNumber = room.RoomNumber;
        }

        public string Capacity { get; set; }

        public int AvailableBeds { get; set; }

        public override string ToString()
        {
            return HotelName + " / " + RoomNumber;
        }
    }
}
