using NepalHajjCommittee.Database.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NepalHajjCommittee.Models
{
    public class BatchDTO : Batch
    {
        public int PeopleCount { get; set; }
    }
}
