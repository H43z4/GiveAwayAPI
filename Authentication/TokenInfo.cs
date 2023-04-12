using System;
using Microsoft.AspNetCore.Authentication;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.Identity;

namespace Authentication
{
    public class TokenInfo
    {
        public TokenInfo() {}

        public TokenInfo(AuthenticationTicket ticket) 
        {
            CreatedAt = DateTime.Now;
            Ticket = ticket;
        }
        
        public DateTime CreatedAt { get; set; }
        
        public AuthenticationTicket Ticket { get; set; }

        public VwUser User { get; set; }
        public VwDSUser DSUser { get; set; }
    }
}