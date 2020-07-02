using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApi.Repository
{
   public class PasswordDecryption
    {

        const int PROVIDER_RSA_FULL = 1;
        const string CONTAINER_NAME = "Tracker";

        public static string decryptPassword(string EncryptedPassword )
        {
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            RSACryptoServiceProvider rsa1 = new RSACryptoServiceProvider(cspParams);
            rsa1.FromXmlString("<RSAKeyValue><Modulus>0Ww1U94lG1ZKBTdIOgICT7hRhMRpb4KtAKQMOldTqTmm8Lc1AWlb07yy/LcTyZXvguhW7j8fZnXOISasMBEOllgKbPYt2lCeZYgcoeqTr3PtgSRYH2xyfLwqJ6Axpd7PlQoZ33UvZjX3ILizrAPPTKEffLxBxSmyB6O2nj5+Pk8=</Modulus><Exponent>AQAB</Exponent><P>6OGIK7oGxJR2lbb/o+8Tp4sKClsXrC/RVCZbbVilYgFX+md32p83g33IUPsuRFKdNiU267SeKWxQ78Jz16tSlw==</P><Q>5jaBEdIsvmDAQ8ESewrhujw8bT3NHYAfs7bg9yLr6OU/0lCfStEY4Tm8VsWalW9MkZ/93K3H/bbTa2+g0oBBCQ==</Q><DP>2Yp0TYSE9fVJoSlFPLoEPiofwvS89FfmzxjVpxt7wkVHDd7BkzPQA8Dn8t3wploWCZJR73TVmhRYtt+KnxinGQ==</DP><DQ>HnrW6eoe3HHo4s+cR8gwDL1O7tPA5YgCKoQrEu1/uI0pvwnHnVtD5QYE5qQxDFn5FErPjLlBglkDcvULoPM0KQ==</DQ><InverseQ>yJBXNxXh2thpO2wT67feil5ehVcD1mq9jTdy64zQ8m5+ud6l3BNOzRDEh9upphtZ7OZbd0fNwe6p+fcj7kHUkA==</InverseQ><D>dizvstHqcpCw2Fynv0Qe9cl3BsqnDKva4D+yPsHEjfvgfnmDybYggU08W2scGWcobuCZHXw1ReY4WXCvPbiCV8MntKodXjkzvQoEBrjHmL5hID8W8gmiap9lwGBDkT0d/rlCknydwbwAfxr5VMMIJr2sOmodpEmXRB5J3DD8rSE=</D></RSAKeyValue>");


            byte[] encyrptedBytes = Convert.FromBase64String(EncryptedPassword);

            byte[] plain = rsa1.Decrypt(encyrptedBytes, false);
            string decryptedString = System.Text.Encoding.UTF8.GetString(plain);
            return decryptedString;
        }
       
     

    }
}
