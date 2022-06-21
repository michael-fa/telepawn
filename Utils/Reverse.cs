using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telepawn.Utils
{
    public static class ReverseString
    {
        public static string Reverse(string s)
        {
            string result = String.Empty;
            char[] cArr = s.ToCharArray();
            int end = cArr.Length - 1;

            for (int i = end; i >= 0; i--)
            {
                result += cArr[i];
            }
            return result;
        }
    }
}
