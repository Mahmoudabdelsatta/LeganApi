using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace CommitteeApi.Repository
{
    public static class Decrypt
    {
       public static (RSAParameters publicKey, RSAParameters privateKey) GenerateKeys(int keyLength)
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = keyLength;
                return (
                    publicKey: rsa.ExportParameters(includePrivateParameters: false),
                    privateKey: rsa.ExportParameters(includePrivateParameters: true)
                );
            }
        }
        public static byte[] EncryptParameter(byte[] data, RSAParameters publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);

                var result = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                return result;
            }
        }
        public static byte[] DecryptParameter(byte[] data, RSAParameters privateKey)
        {
            const int PROVIDER_RSA_FULL = 1;
            const string CONTAINER_NAME = "Tracker";

            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            RSACryptoServiceProvider rsa1 = new RSACryptoServiceProvider(cspParams);
            rsa1.ImportParameters(privateKey);
           return rsa1.Decrypt(data, false);
           
        }
    }
}