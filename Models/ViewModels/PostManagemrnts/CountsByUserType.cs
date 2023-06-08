using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.PostManagemrnts
{
    public class CountsByUserType
    {
        public long IndividualCounts { get; set; }
        public long OrganizationalCounts { get; set; }
        public long OtherCounts { get; set; }
    }
}
