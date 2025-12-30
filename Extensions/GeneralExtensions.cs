using System.Text;
using System.Text.RegularExpressions;

namespace ClearStore.Extensions
{
    public static class GeneralExtensions
    {
        public static string ToCurrency(this decimal value)
        {
            return string.IsNullOrEmpty(value.ToString()) ? string.Empty : value.ToString("C2");
        }


        public static string ToShortCurrency(this decimal value)
        {
            if (value >= 1000000)
            {
                return $"${value / 1000000:F1}M";
            }
            else if (value >= 1000)
            {
                return $"${value / 1000:F0}k";
            }

            return value.ToString("C2");
        }


        public static string ToPhoneNumber(this string phoneNumber, bool? includeParens = false)
        {
            string pattern = @"(\d{3})(\d{3})(\d{4})";
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                string format = "$1-$2-$3";
                if (includeParens.HasValue && includeParens == true)
                {
                    format = "$(1) $2-$3";
                }
                string newPhoneNumber = Regex.Replace(phoneNumber, pattern, format);
                return newPhoneNumber;
            }
            return string.Empty;
        }


        public static string GetImageContentType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "application/octet-stream";
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        public static string SanitizeName(string name)
        {
            char[] chars = Path.GetInvalidFileNameChars();

            foreach (var c in chars)
            {
                name = name.Replace(c, '_');
            }

            name = Regex.Replace(name, @"\s+", "_").Trim();

            return name;
        }


        public static string SanitizeCsv(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = input.Normalize(NormalizationForm.FormD);
            string sanitized = Regex.Replace(input, @"[^\u0000-\u007F]+", "-");

            return sanitized;
        }


        public static string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
            {
                return $"\"{input.Replace("\"", "\"\"")}\"";
            }

            return input;
        }
    }
}
