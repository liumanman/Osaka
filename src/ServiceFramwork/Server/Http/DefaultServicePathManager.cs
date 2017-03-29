using System.Text.RegularExpressions;

namespace ServiceFramwork.Server.Http
{
    public class DefaultServicePathManager : IServicePathManager
    {
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

        private static string ConvertToRegexPattern(string urlPattern)
        {
            return Regex.Replace(urlPattern, @"{(\w+)}", @"(?<$1>\w+)");
        }
    }
}
