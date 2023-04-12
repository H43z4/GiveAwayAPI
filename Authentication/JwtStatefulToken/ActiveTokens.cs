using Authentication.JwtStatefulToken;
using Models.ViewModels.Identity;
using System.Collections.Concurrent;
using System.Linq;

namespace Authentication.JwtStatefulToken
{
    public static class ActiveTokens
    {
        public static ConcurrentDictionary<string, TokenInfo> _tokens = new ConcurrentDictionary<string, TokenInfo>();
        
        public static VwUser GetUser(int userId)
        {
            return _tokens.Values.SingleOrDefault(x => x.User.UserId == userId).User;
        }
    }
}
