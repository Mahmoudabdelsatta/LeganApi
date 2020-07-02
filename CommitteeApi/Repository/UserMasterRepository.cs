using CommitteeApi.Models;
using System;
using System.Linq;

namespace CommitteeApi.Models
{
    public class UserMasterRepository: IDisposable
    {
        // SECURITY_DBEntities it is your context class
        Committee_DBEntities1 context = new Committee_DBEntities1();

       

        //This method is used to check and validate the user credentials
        public User ValidateUser(string Phone)
        {
                return context.Users.FirstOrDefault(user =>
                user.Phone.ToString().Equals(Phone, StringComparison.OrdinalIgnoreCase)
              );
            }
        
        public void Dispose()
        {
            context.Dispose();
        }
    }
}