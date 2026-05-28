using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infra.CrossCutting.Utilities
{
    public static class HashHelper
    {
        public static string Compute(params object?[] values)
        {
            var normalized =
                string.Join("|", values.Select(Normalize));

            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(normalized);
            var hash = sha.ComputeHash(bytes);

            return ConvertToHex(hash);
        }

        private static string Normalize(object? value)
        {
            if (value is null)
                return "null";

            if (value is DateTime dt)
                return dt.ToUniversalTime().ToString("O");

            return value.ToString() ?? "null";
        }

        private static string ConvertToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
