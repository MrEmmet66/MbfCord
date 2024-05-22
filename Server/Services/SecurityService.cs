using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
	internal class SecurityService
	{
		private readonly string key;

		public SecurityService(string key)
		{
			this.key = key;
		}

		public string EncryptPassword(string password)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(key);
				aesAlg.IV = new byte[16];

				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				byte[] encryptedBytes;
				using (var msEncrypt = new MemoryStream())
				{
					using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (var swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(password);
						}
						encryptedBytes = msEncrypt.ToArray();
					}
				}

				return Convert.ToBase64String(encryptedBytes);
			}
		}

		public string DecryptPassword(string encryptedPassword)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(key);
				aesAlg.IV = new byte[16];

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
				string decryptedPassword;
				using (var msDecrypt = new MemoryStream(encryptedBytes))
				{
					using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (var srDecrypt = new StreamReader(csDecrypt))
						{
							decryptedPassword = srDecrypt.ReadToEnd();
						}
					}
				}

				return decryptedPassword;
			}
		}
	}
}
