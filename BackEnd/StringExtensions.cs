using System;
using System.Collections.Generic;
using System.IO;

namespace BackEnd
{
    public static class DateTimeExtensions
    {
        public static long ToTimestamp(this DateTime dateTime)
        {
            return Convert.ToInt64(dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
        }
    }
    
    
    public static class StringExtensions
    {
        public static List<string> GetAllLines(this string source)
        {
            var lines = new List<string>();
            using (var reader = new StringReader(source))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }

            return lines;
        }

        public static T GetValueAs<T>(this String value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}