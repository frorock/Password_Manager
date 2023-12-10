namespace Password.Services
{
    public class EvaluatePasswordStrength
    {
        public enum PasswordStrength
        {
            Low,
            Medium,
            High,
            VeryHigh
        }

        public static PasswordStrength CheckStrength(string password)
        {
            int score = 0;

            if (string.IsNullOrWhiteSpace(password))
                return PasswordStrength.Low;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;

            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?"))
                score++;

            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*()\-_=+{};:,<.>?@[]"))
                score++;

            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]") && System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
                score++;

            if (score < 2)
                return PasswordStrength.Low;
            if (score < 3)
                return PasswordStrength.Medium;
            if (score < 4)
                return PasswordStrength.High;

            return PasswordStrength.VeryHigh;
        }
    }
}
