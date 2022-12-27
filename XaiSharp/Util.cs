using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace XaiSharp
{
    public class Util
    {

        public class Commit
        {
            public string Sha { get; set; }
            public CommitDetail CommitDetail { get; set; }
            public Author Author { get; set; }
            public Author Committer { get; set; }
            public string Message { get; set; }
            public List<Parent> Parents { get; set; }
        }

        public class CommitDetail
        {
            public Author Author { get; set; }
            public Author Committer { get; set; }
            public string Message { get; set; }
            public Tree Tree { get; set; }
            public List<string> Verification { get; set; }
        }

        public class Author
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTimeOffset Date { get; set; }
        }

        public class Parent
        {
            public string Sha { get; set; }
            public string Url { get; set; }
            public string HtmlUrl { get; set; }
        }

        public class Tree
        {
            public string Sha { get; set; }
            public string Url { get; set; }
        }


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
