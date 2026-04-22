using Domain.Enum;
using Microsoft.AspNetCore.Http;

namespace API.Common.Language
{
    public static class LanguageResolver
    {
        public const string QueryKey = "lang";

        public static LanguageCode Resolve(HttpRequest request)
        {
            if (request.Query.TryGetValue(QueryKey, out var q) && TryParse(q.ToString(), out var fromQuery))
                return fromQuery;

            var accept = request.Headers.AcceptLanguage.ToString();
            if (TryParseAcceptLanguage(accept, out var fromHeader))
                return fromHeader;

            return LanguageCode.Vi;
        }

        public static bool TryParse(string? value, out LanguageCode lang)
        {
            lang = LanguageCode.Vi;
            var s = (value ?? string.Empty).Trim().ToLowerInvariant();
            if (s is "vi" or "vi-vn") { lang = LanguageCode.Vi; return true; }
            if (s is "en" or "en-us" or "en-gb") { lang = LanguageCode.En; return true; }
            return false;
        }

        private static bool TryParseAcceptLanguage(string? header, out LanguageCode lang)
        {
            // Typical: "en-US,en;q=0.9,vi;q=0.8"
            lang = LanguageCode.Vi;
            var s = (header ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(s)) return false;

            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var p in parts)
            {
                var token = p.Split(';', 2, StringSplitOptions.TrimEntries)[0];
                if (TryParse(token, out lang)) return true;
            }

            return false;
        }
    }
}

