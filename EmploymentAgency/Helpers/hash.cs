using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentAgency.Helpers
{
    class hash
    {
        public static string hashString(string str)
        {
            string hashed = "";
            using (var hash = SHA256.Create())
            {
                byte[] bhash = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
                for (int i = 0; i < bhash.Length; i++)
                {
                    hashed += bhash[i].ToString("X2");
                }
            }
            return hashed;
        }
    }
}
