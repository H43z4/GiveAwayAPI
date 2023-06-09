using Models.ViewModels.DSAuth.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.PostManagemrnts
{
    public class UserWithPostsVM
    {
        //public UserVM User { get; set; }
        public long userId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public List<ResponsePostVM> Posts { get; set; }
        public List<ResponsePostVM> ReqPosts { get; set; }

    }
}
