
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApi.Repository
{
   public class HypridEncryption
    {
        public static string decrypt(string encryptedParameter,string encryptedPassword )
        {
            string password = PasswordDecryption.decryptPassword(encryptedPassword);
            string parameter = ParamaterDecryption.DecryptParamater(encryptedParameter, password);
            return parameter;

        }
    }
}
