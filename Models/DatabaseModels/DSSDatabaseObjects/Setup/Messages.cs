using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class Messages
    {
        [Key]
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public bool Status { get; set; }

    }
}
