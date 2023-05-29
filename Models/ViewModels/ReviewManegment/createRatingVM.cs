using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.ReviewManegment
{
    public class createRatingVM
    {
        public int Id { get; set; }
        public int RecevedReviewUserId { get; set; }
        public int SendReviewUserId { get; set; }
        public int PostId { get; set; }
        public int ProfileRating { get; set;}
        public int PostRating { get; set;}       

    }
}
