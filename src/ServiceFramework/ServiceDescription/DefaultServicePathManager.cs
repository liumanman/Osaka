using System;
using System.Text.RegularExpressions;

namespace ServiceFramework.ServiceDescription
{
    public class DefaultServicePathManager : IServicePathManager
    {
        const String PLACEHOLDER_SERVICE = "{service}";
        const String PLACEHOLDER_OPERATION = "{operation}";

        string _basePath;
        public DefaultServicePathManager(string basePath)
        {
            _basePath = basePath;
        }

        public bool IsMatch(string path, string pattern)
        {
            string fullPattern = pattern.StartsWith("/") ? pattern : _basePath + pattern;
            string regexPattern = ConvertToRegexPattern(fullPattern);
            return Regex.IsMatch(path, regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public string GetServicePath(string serviceName, string operationName, string pattern)
        {
            var path = Regex.Replace(pattern, PLACEHOLDER_SERVICE, serviceName, RegexOptions.IgnoreCase);
            return Regex.Replace(path, PLACEHOLDER_OPERATION, operationName, RegexOptions.IgnoreCase);
        }

        private static string ConvertToRegexPattern(string urlPattern)
        {
            return Regex.Replace(urlPattern, @"{(\w+)}", @"(?<$1>\w+)");
        }
    }
}
