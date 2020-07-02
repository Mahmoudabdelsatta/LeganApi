using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using CommitteeApi.Models;

namespace CommitteeApi.Repository
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public string userPhone = "";
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            using (UserMasterRepository _repo = new UserMasterRepository())
            {
                //var user = _repo.ValidateUser(context.Password);

                userPhone = context.Parameters.Where(x => x.Key == "phone").Select(x => x.Value).FirstOrDefault()[0].ToString();

                var user = _repo.ValidateUser(userPhone);
                if (user == null)
                {
                    context.SetError("invalid_grant", "Provided  password is incorrect");
                    return ;
                }
                
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Role, user.SystemRoleMap.titleEn));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                //identity.AddClaim(new Claim("UserEmailId", user.UserEmailId));
                //identity.AddClaim(new Claim("Phone", user.Phone));
                identity.AddClaim(new Claim("ID", user.ID.ToString()));

                context.Validated("ID");
            }
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (UserMasterRepository _repo = new UserMasterRepository())
            {
                //var user = _repo.ValidateUser(context.Password);
                string uid = context.OwinContext.Get<string>("phone");
                string uid2 = context.ClientId;
                var uid3 = context.Response.Headers.ContainsKey("phone");

                var user = _repo.ValidateUser(userPhone);
                if (user == null)
                {
                    context.SetError("invalid_grant", "Provided  password is incorrect");
                    return;
                }
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Role, user.SystemRoleMap.titleEn));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                //identity.AddClaim(new Claim("UserEmailId", user.UserEmailId));
                //identity.AddClaim(new Claim("Phone", user.Phone));
                identity.AddClaim(new Claim("ID", user.ID.ToString()));
                context.Validated(identity);
            }
        }

    }
}