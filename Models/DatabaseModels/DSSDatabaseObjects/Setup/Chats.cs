using System.ComponentModel.DataAnnotations;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class Chats
    {
        [Key]
        public int ChatId { get; set; }
        public int PostId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public bool ReadStatus { get; set; }
    }
}
