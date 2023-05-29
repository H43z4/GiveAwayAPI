namespace Models.ViewModels.ChatManegement
{
    public class SendMessegeVM
    {
        public int PostId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public string Messege { get;set; }  
    }
}
