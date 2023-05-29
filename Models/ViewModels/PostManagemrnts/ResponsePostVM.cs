using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.ViewModels.PostManagemrnts
{
    public class ResponsePostVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int CategoryId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string PostDiscription { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public DateTime ValidDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int ItemSize { get; set; }
        public string Location { get; set; }
        [JsonIgnore]
        public List<string> Images { get; set; }
    }
}
