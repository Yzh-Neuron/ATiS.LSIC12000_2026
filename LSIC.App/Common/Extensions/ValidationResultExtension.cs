namespace LSIC.App
{
    internal static class ValidationResultExtension
    {
        public static string ToMessageString(this IEnumerable<ValidationResult> errors)
        {
            var lines = errors.Select(error =>
            {
                string members = string.Join(">", error.MemberNames ?? Enumerable.Empty<string>());
                if (members.Length > 0)
                {
                    return $"{members}: {error.ErrorMessage}";
                }
                return error.ErrorMessage;
            });
            return string.Join(Environment.NewLine, lines);
        }
    }
}
