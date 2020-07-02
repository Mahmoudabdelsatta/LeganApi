using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace CommitteeApi.Repository
{
    public static class ParamaterDecryption
    {
        private static string IV = "12345678rtef4321";
        private static string SALT = "[poiuyhjlkmakscl;klhajflksdk;fjsljfnldhjdf";

        public static string DecryptParamater(string encrypted,string password)
        { using (var csp = new AesCryptoServiceProvider())
            {
                var d = GetCryptoTransform(csp, false,password);
                byte[] output = Convert.FromBase64String(encrypted);
                byte[] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);
                string decypted = Encoding.UTF8.GetString(decryptedOutput);
                return decypted;
            }
        }

        private static ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting,string password)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(SALT), 65536);
            byte[] key = spec.GetBytes(16);


            csp.IV = Encoding.UTF8.GetBytes(IV);
            csp.Key = key;
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }
    }
}