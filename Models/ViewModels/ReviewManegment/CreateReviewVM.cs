using Models.DatabaseModels.DSSDatabaseObjects.Setup;
using Models.ViewModels.PostManagemrnts;
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
        public string ReceverUserName { get; set; }
        public string SenderUserName { get; set; }
        public int SenderUserId { get; set; }
        public ResponsePostVM? Post { get; set; } = null;
        //public string Status { get; set; }

    }
}
