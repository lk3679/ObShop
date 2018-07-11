using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace OBShopWeb.Poslib
{
    public class Cryptography
    {
        public static string decode(string plainStr)
        {
            System.Security.Cryptography.AesManaged aaa = new System.Security.Cryptography.AesManaged();
            aaa.Mode = System.Security.Cryptography.CipherMode.CBC;
            aaa.KeySize = 256;
            aaa.BlockSize = 128;
            aaa.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            string keyStr = "cGFzc3dvcmQAejABCAAAAA==";
            string ivStr = "cGFzc3dvcmQAAAenAAABCA==";
            byte[] ivArr = Convert.FromBase64String(keyStr);
            byte[] keyArr = Convert.FromBase64String(ivStr);
            aaa.IV = ivArr;
            aaa.Key = keyArr;

            // This array will contain the plain text in bytes
            byte[] plainText = Convert.FromBase64String(plainStr);

            // Creates Symmetric encryption and decryption objects   
            System.Security.Cryptography.ICryptoTransform decrypto = aaa.CreateDecryptor();
            // The result of the encrypion and decryption
            byte[] decryptedText = decrypto.TransformFinalBlock(plainText, 0, plainText.Length);

            string decryptedString = ASCIIEncoding.UTF8.GetString(decryptedText);
            return decryptedString;
            

        }
    }
}