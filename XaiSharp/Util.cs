﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using YamlDotNet.Serialization;
namespace XaiSharp
{
    public class Util
    {
        public static Config ParseConfig(string ymlContents)
        {
            var deserializer = new DeserializerBuilder()
                //.WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<Config>(ymlContents);
        }

        public static string ToUnixTime(DateTimeOffset? dateTime)
        {
            DateTimeOffset dto = (DateTimeOffset)dateTime;
            return dto.ToUnixTimeSeconds().ToString();
        }

        public static string CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes); // .NET 5 +

                // Convert the byte array to hexadecimal string prior to .NET 5
                // StringBuilder sb = new System.Text.StringBuilder();
                // for (int i = 0; i < hashBytes.Length; i++)
                // {
                //     sb.Append(hashBytes[i].ToString("X2"));
                // }
                // return sb.ToString();
            }
        }

        public static string CreateDateString(string year, string month, string day)
        {
            return $"{year}-{month.PadLeft(2, '0')}-{day.PadLeft(2, '0')}";
        }
    }
}
