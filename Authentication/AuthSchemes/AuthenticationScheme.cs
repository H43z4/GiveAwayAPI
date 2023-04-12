namespace Authentication.AuthSchemes
{
    public static class AuthenticationSchemes
    {
        public const string JWT_BEARER_TOKEN = "DefaultTokenAuthenticationScheme";
        public const string JWT_BEARER_TOKEN_STATELESS = "JwtStatelessTokenAuthenticationScheme";
        public const string JWT_BEARER_TOKEN_IP_BASED = "UserWithIPTokenAuthenticationScheme";
    }
}
