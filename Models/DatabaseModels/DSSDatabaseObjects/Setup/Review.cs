using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int PostId { get; set; }
        public int ReceverUserId { get; set; }
        public int SenderUserId { get; set; }
        public string Status { get; set; }
    }
}
