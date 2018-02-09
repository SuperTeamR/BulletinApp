using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BulletinTools
{
    public static class Сryptography
    {
        /// <summary>
        /// Конвертирую строку в байт массив шифрованный с помощью SHA256
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] StringToSha256ByteArray(string text)
        {
            var bytes = Encoding.Unicode.GetBytes(text);
            var hashstring = new SHA256Managed();
            var hash = hashstring.ComputeHash(bytes);
            return hash;
        }
        /// <summary>
        /// Конвертирую строку в Hash строку шифрованную с помощью 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StringToSha256String(params string[] str)
        {
            var temp = string.Empty;
            foreach (var s in str)
            {
                temp += s ?? string.Empty;
            }
            return StringToSha256String(temp);
        }
        /// <summary>
        /// Конвертирую строку в Hash строку шифрованную с помощью SHA256
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StringToSha256String(string text)
        {
            var hash = StringToSha256ByteArray(text);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
        /// <summary>
        /// Конвертирую строку в массив в байт в строку 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] bytes)
        {
            string hashString = string.Empty;
            foreach (byte x in bytes)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
