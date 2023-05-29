using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.ReviewManegment
{
    public class CreateReviewVM
    {
        public int PostId { get; set; }
        public int ReceverUserId { get; set; }
        public int SenderUserId { get; set; }
        //public string Status { get; set; }
    }
}
