namespace LSIC.App
{
    internal static class ValidationResultExtension
    {
        public static string ToMessageString(this IEnumerable<ValidationResult> errors)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ValidationResult error in errors)
            {
                builder.AppendFormat("{0}{1}{2}\n", string.Join(">", error.MemberNames), ": ", error.ErrorMessage);
            }
            return builder.ToString();
        }
    }
}
