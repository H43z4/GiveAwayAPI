using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.PostManagemrnts
{
    public class CountsByCategory
    {
        public long WantedCounts { get; set; }
        public long NonFoodCounts { get; set; }
        public long BorrowCounts { get; set; }
        public long FoodCounts { get; set; }
        //public int TotalCount { get; set; }
    }
}
