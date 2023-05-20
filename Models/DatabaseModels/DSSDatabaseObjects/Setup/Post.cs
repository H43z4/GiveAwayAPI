using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string PostDiscription { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public DateTime CreatePost { get; set; }  
        public  bool IsActive { get; set; }
        public DateTime UpdatePost { get; set; }
        public DateTime ValidDate { get; set; }
        public int ItemSize { get; set; }
        public string Location { get; set; }        
    }
}
