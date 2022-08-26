using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SachDienTu.Common
{
    public static class StringExtensions
    {
        public static string TextToUrl(this string text)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string url = text.Normalize(NormalizationForm.FormD).Trim().ToLower();

            url = regex.Replace(url, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace(",", "-").Replace(".", "-")
                        .Replace("!", "").Replace("(", "").Replace(")", "").Replace(";", "-").Replace("/", "-")
                        .Replace("%", "ptram").Replace("&", "va").Replace("?", "").Replace('"', '-').Replace(' ', '-');
            return url;
        }

        public static string RemoveHtmlTag(this string strHtml)
        {
            if (strHtml == null)
                return null;
            return Regex.Replace(strHtml, "<[^>]*>", string.Empty);
        }

        public static string ShortDesc(this string description, int length)
        {
            if (string.IsNullOrEmpty(description))
                return "...";
            if (description.Length <= length)
                return $"{description}...";
            return $"{description.Substring(0, length)}...";
        }
    }
}