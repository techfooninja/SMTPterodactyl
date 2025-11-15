namespace SMTPterodactyl.Utilities
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static Type? GetTypeByClassName(string? className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ArgumentException("Class name cannot be null or empty.", nameof(className));
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly
                        .GetTypes()
                        .FirstOrDefault(t => string.Equals(t.Name, className, StringComparison.OrdinalIgnoreCase));

                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var type = ex.Types
                        .FirstOrDefault(t => t != null && string.Equals(t.Name, className, StringComparison.OrdinalIgnoreCase));

                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}
