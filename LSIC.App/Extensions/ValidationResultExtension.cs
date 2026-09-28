namespace LSIC.App
{
    internal static class ValidationResultExtension
    {
        public static string ToMessageString(this IEnumerable<ValidationResult> errors)
        {
            StringBuilder builder = new StringBuilder();

            foreach (ValidationResult error in errors)
            {
                string members = string.Join(">", error.MemberNames ?? Enumerable.Empty<string>());

                if (members.Length > 0)
                {
                    builder.Append(members).Append(": ");
                }

                builder.Append(error.ErrorMessage).Append('\n');
            }

            return builder.ToString();
        }
    }
}
