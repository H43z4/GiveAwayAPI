﻿using Models.ViewModels.DSAuth.Setup;

namespace Models.ViewModels.Identity
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }

        public string Token { get; set; }

        public VwUser User { get; set; }

        public VwDSUser DSUser { get; set; }
    }
}
