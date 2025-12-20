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
            // vnp_SecureHashType must be sent to VNPAY but excluded from the signature
            var orderedRequest = _requestData.OrderBy(x => x.Key, StringComparer.Ordinal);

            var query = string.Join("&", orderedRequest.Select(x =>
                $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

            var signData = BuildRequestSignData();
            var hash = HmacSHA512(hashSecret, signData);

            return $"{baseUrl}?{query}&vnp_SecureHash={hash}";
        }

        public bool ValidateSignature(string inputHash, string secret)
        {
            var signData = BuildResponseSignData();

            var hash = HmacSHA512(secret, signData);
            return hash.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
        }

        public string BuildRequestSignData()
        {
            var signedRequestData = _requestData
                .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
                .OrderBy(x => x.Key, StringComparer.Ordinal);

            return string.Join("&", signedRequestData.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));
        }

        public string BuildResponseSignData()
        {
            var signedResponseData = _responseData
                .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
                .OrderBy(x => x.Key, StringComparer.Ordinal);

            return string.Join("&", signedResponseData.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));
        }

        private static string HmacSHA512(string key, string input)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(input)))
                .Replace("-", "").ToUpperInvariant();
        }
    }

}
