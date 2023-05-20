using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.PostManagemrnts
{
    public class CreatePostVM
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string PostDiscription { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public DateTime ValidDate { get; set; }
        public int ItemSize { get; set; }
        public string Location { get; set; }
        public List<IFormFile> Images { get; set; }

    }
}
