namespace TestJob.Extensions
{
    public static class EnvironmentExtensions
    {
        public static string ParseStringFromEnvironmentVariable(string variableName, string defaultValue)
        {
            string? envValue = Environment.GetEnvironmentVariable(variableName);
            if (envValue is not null)
            {
                return envValue;
            }

            return defaultValue;
        }

        public static int ParseIntFromEnvironmentVariable(string variableName, int defaultValue)
        {
            string? envValue = Environment.GetEnvironmentVariable(variableName);
            if (int.TryParse(envValue, out int result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
