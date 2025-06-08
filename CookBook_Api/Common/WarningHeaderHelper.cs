using CookBook_Api.Common.ErrorHandling;

namespace CookBook_Api.Common
{
    public static class WarningHeaderHelper
    {
        private const string HEADER_WARNING_NAME = "X-Warnings";

        public static void AddWarningsToResponse(HttpResponse response, IEnumerable<Error> warnings)
        {
            if (warnings == null || !warnings.Any())
                return;

            var wariningMessages = string.Join(" | ", warnings.Select(w => w.Message));
            response.Headers[HEADER_WARNING_NAME] = wariningMessages;
        }
    }
}
