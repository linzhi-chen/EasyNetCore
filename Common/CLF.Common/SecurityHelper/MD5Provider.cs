using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CLF.Common.SecurityHelper
{
    public class MD5Provider
    {
        private MD5Provider()
        {
        }
        /// <summary>
        /// 计算指定字符串的MD5哈希值
        /// </summary>
        /// <param name="message">要进行哈希计算的字符串</param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static string Hash(string message, bool isLower = true)
        {
            if (string.IsNullOrEmpty(message))
            {
                return string.Empty;
            }
            else
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                Byte[] fromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(message);
                Byte[] targetData = md5.ComputeHash(fromData);
                string byte2String = "";
                for (int i = 0; i < targetData.Length; i++)
                {
                    byte2String += targetData[i].ToString("x2");
                }
                if (isLower)
                    return byte2String.ToLower();
                return byte2String.ToUpper();
            }
        }

        /// <summary>
        /// 字符串计算hash值转换成19位长整型数字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ulong Hash(string key)
        {
            using (var hash = MD5.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                var a = BitConverter.ToUInt64(data, 0);
                var b = BitConverter.ToUInt64(data, 8);
                ulong hashCode = a ^ b;
                return hashCode;
            }
        }
    }
}
