using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;


namespace APIGateway.Common
{
    public class ClsCommon
    {
        public static int GetCurrentUserId(ControllerBase controller)
        {
                try
                {
                    var claimsIdentity = controller.HttpContext.User.Identity as ClaimsIdentity;

                    var Claim = claimsIdentity.FindFirst("UserId");

                    return Convert.ToInt32(Claim.Value);
                }
                catch
                {
                    return 0;
                }
        }


    }
 
}
