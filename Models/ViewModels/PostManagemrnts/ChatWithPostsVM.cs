using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.PostManagemrnts
{
    public class ChatWithPostsVM
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public int ReceiverId { get; set; } 
        public string ReceiverName { get; set; } 
        public List<conversations> conversations { get; set; }

    }
    public class conversations
    {
        public string senderUserName { get; set; }=string.Empty;
        public string Msg { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }
}
