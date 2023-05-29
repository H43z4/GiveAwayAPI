using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels.DSSDatabaseObjects.Setup;
using Models.ViewModels.ChatManegement;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.PostManagemrnts;
using RepositoryLayer;
using SharedLib.Interfaces;

namespace ChatManagement
{
    public interface IChatServise: ICurrentDSUser
    {
        Task<int> PostMassege(SendMessegeVM SendMessege);
        Task<List<ChatWithPostsVM>> ChatsWithUserID();
        Task<ChatWithPostsVM> ChatsWithUserIdOnPost(int userId, int postId);
        Task<Chats> Chatbox(SendMessegeVM SendMessege);
    }
    public class ChatServices : IChatServise
    {
        private readonly AppDbContext _context;
        public VwDSUser VwUser { get; set; }
        public VwDSUser VwDSUser { get; set; }
        public ChatServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatWithPostsVM> ChatsWithUserIdOnPost(int userId, int postId)
        {
            var senderId = (int)VwDSUser.UserId;
            var chkexist = await _context.Chat.Where(x => (x.SenderId == userId || x.ReceiverId == userId)&& x.PostId==postId).FirstOrDefaultAsync();
            if (chkexist == null)
                return null;            
            ChatWithPostsVM temp = new ChatWithPostsVM();
            temp.PostId = chkexist.PostId;
            temp.SenderId = chkexist.SenderId;
            temp.ReceiverId = chkexist.ReceiverId;
            temp.PostTitle = await _context.Posts.Where(x => x.Id == postId).Select(x => x.PostTitle).FirstOrDefaultAsync();
            temp.conversations = new List<conversations>(); // Initialize the conversations list

            var conversation = await _context.Message.Where(x => x.ChatId == chkexist.ChatId).ToListAsync();

            if (conversation.Count > 0)
            {
                foreach (var conv in conversation)
                {
                    conversations c = new conversations();
                    if (conv.SenderId == senderId)
                    {
                        c.senderUserName = await _context.User.Where(x => x.UserId == senderId).Select(x => x.UserName).FirstOrDefaultAsync();
                        c.Msg = conv.Message;
                        c.DateTime = conv.DateTime;
                    }
                    else
                    {
                        c.Msg = conv.Message;
                        c.DateTime = conv.DateTime;
                    }

                    temp.conversations.Add(c);
                }
            }
            return temp;
        }

        public async Task<List<ChatWithPostsVM>> ChatsWithUserID()
        {
            var senderId = (int)VwDSUser.UserId;
            var chkexist = await _context.Chat.Where(x => x.SenderId == senderId || x.ReceiverId == senderId).ToListAsync();
            if (chkexist.Count == 0)
                return null;

            List<ChatWithPostsVM> data = new List<ChatWithPostsVM>();

            foreach (var chat in chkexist)
            {
                ChatWithPostsVM temp = new ChatWithPostsVM();
                temp.PostId = chat.PostId;
                temp.SenderId = chat.SenderId == senderId?chat.SenderId:chat.ReceiverId;
                temp.SenderUserName = await _context.User.Where(x => x.UserId == temp.SenderId).Select(x => x.UserName).FirstOrDefaultAsync();
                temp.ReceiverId = chat.ReceiverId == senderId ? chat.SenderId : chat.ReceiverId;
                temp.ReceiverName = await _context.User.Where(x => x.UserId == temp.ReceiverId).Select(x => x.FullName).FirstOrDefaultAsync();
                temp.PostTitle = await _context.Posts.Where(x => x.Id == chat.PostId).Select(x => x.PostTitle).FirstOrDefaultAsync();

                temp.conversations = new List<conversations>(); // Initialize the conversations list

                var conversation = await _context.Message.Where(x => x.ChatId == chat.ChatId).ToListAsync();

                if (conversation.Count > 0)
                {
                    foreach (var conv in conversation)
                    {
                        conversations c = new conversations();
                        if (conv.SenderId == senderId)
                        {
                            c.senderUserName = await _context.User.Where(x => x.UserId == senderId).Select(x => x.UserName).FirstOrDefaultAsync();
                            c.Msg = conv.Message;
                            c.DateTime = conv.DateTime;
                        }
                        else
                        {
                            c.senderUserName = await _context.User.Where(x => x.UserId == conv.SenderId).Select(x => x.UserName).FirstOrDefaultAsync();
                            c.Msg = conv.Message;
                            c.DateTime = conv.DateTime;
                        }

                        temp.conversations.Add(c);
                    }
                }

                data.Add(temp);
            }

            return data;
        }

        //public async Task<List<ChatWithPostsVM>> ChatsWithUserID(int userId)
        //{
        //    var chkexist = await _context.Chat.Where(x=>x.SenderId == userId || x.ReceiverId==userId).ToListAsync();
        //    if (chkexist == null)
        //        return null;
        //    List<ChatWithPostsVM> data = new List<ChatWithPostsVM>();
        //    foreach (var chat in chkexist)
        //    {
        //        ChatWithPostsVM temp = new ChatWithPostsVM();
        //        temp.SenderId = chat.SenderId;
        //        temp.ReceiverId = chat.ReceiverId;
        //        var conversation= await _context.Message.Where(x=>x.ChatId==chat.ChatId).ToListAsync();
        //        if(conversation.Count>0)
        //        {
        //            foreach (var conv in conversation)
        //            {
        //                conversations c = new conversations();
        //                if (conv.SenderId == userId)
        //                {
        //                    c.SendMsg = conv.Message;
        //                    c.DateTime = conv.DateTime;
        //                }
        //                else
        //                {
        //                    c.ReceiveMsg = conv.Message;
        //                    c.DateTime = conv.DateTime;
        //                }

        //                temp.conversations.Add(c);
        //            }
        //        }

        //        data.Add(temp);

        //    }
        //    return data;
        //}

        public async Task<int> PostMassege(SendMessegeVM SendMessege)
        {
            var chkexist =await _context.Chat.Where(x=>((x.SenderId== SendMessege.SenderUserId&& x.ReceiverId == SendMessege.ReceiverUserId) ||( x.SenderId == SendMessege.ReceiverUserId && x.ReceiverId == SendMessege.SenderUserId)) && x.PostId== SendMessege.PostId).FirstOrDefaultAsync();
            if (chkexist ==null)
            {
                Chats obj = new Chats();
                obj.SenderId = SendMessege.SenderUserId;
                obj.ReceiverId= SendMessege.ReceiverUserId;
                obj.PostId = SendMessege.PostId;
                obj.ReadStatus = false;
                _context.Chat.Add(obj);
                await _context.SaveChangesAsync();

                Messages messages = new Messages();
                messages.ChatId = obj.ChatId;
                messages.SenderId = SendMessege.SenderUserId;
                messages.ReceiverId = SendMessege.ReceiverUserId;
                messages.Message = SendMessege.Messege;
                messages.Status = false;
                messages.DateTime=DateTime.Now;
                _context.Message.Add(messages);
                await _context.SaveChangesAsync();
                return 1;
            }
            else
            {
                Messages messages = new Messages();
                messages.ChatId = chkexist.ChatId;
                messages.SenderId = SendMessege.SenderUserId;
                messages.ReceiverId = SendMessege.ReceiverUserId;
                messages.Message = SendMessege.Messege;
                messages.Status = false;
                messages.DateTime = DateTime.Now;

                _context.Message.Add(messages);
                await _context.SaveChangesAsync();
                return 1;
            }
        }
        public async Task<Chats> Chatbox(SendMessegeVM SendMessege)
        {
            SendMessege.SenderUserId = (int)VwDSUser.UserId;
            var chkexist =await _context.Chat.Where(x=>((x.SenderId== SendMessege.SenderUserId&& x.ReceiverId == SendMessege.ReceiverUserId) ||( x.SenderId == SendMessege.ReceiverUserId && x.ReceiverId == SendMessege.SenderUserId)) && x.PostId== SendMessege.PostId).FirstOrDefaultAsync();
            if (chkexist ==null)
            {
                Chats obj = new Chats();
                obj.SenderId = SendMessege.SenderUserId;
                obj.ReceiverId= SendMessege.ReceiverUserId;
                obj.PostId = SendMessege.PostId;
                obj.ReadStatus = false;
                _context.Chat.Add(obj);
                await _context.SaveChangesAsync();
                return obj;
            }
            else
            {
                return chkexist;
            }
        }
    }
}