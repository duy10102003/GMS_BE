using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Helpers.Vnpay
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new();
        private readonly SortedList<string, string> _responseData = new();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData[key] = value;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _responseData[key] = value;
        }

        public string GetResponseData(string key)
            => _responseData.ContainsKey(key) ? _responseData[key] : null;

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var query = string.Join("&", _requestData.Select(x =>
                $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

            var signData = string.Join("&", _requestData.Select(x => $"{x.Key}={x.Value}"));
            var hash = HmacSHA512(hashSecret, signData);

            return $"{baseUrl}?{query}&vnp_SecureHash={hash}";
        }

        public bool ValidateSignature(string inputHash, string secret)
        {
            var signData = string.Join("&", _responseData
                .Where(x => x.Key != "vnp_SecureHash")
                .Select(x => $"{x.Key}={x.Value}"));

            var hash = HmacSHA512(secret, signData);
            return hash.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSHA512(string key, string input)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(input)))
                .Replace("-", "").ToLower();
        }
    }

}
