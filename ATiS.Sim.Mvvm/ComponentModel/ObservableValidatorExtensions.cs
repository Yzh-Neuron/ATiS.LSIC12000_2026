using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ATiS.Sim.Mvvm
{
    public static class ObservableValidatorExtensions
    {
        private static readonly Dictionary<Type, PropertyInfo[]> RecursivePropertiesCache = new();
        public static List<ValidationResult> GetAllErrorsRecursive(this ObservableValidator target)
        {
            ArgumentNullException.ThrowIfNull(target);
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            return GetAllErrorsRecursive(target, visited, true);
        }
        public static List<ValidationResult> GetAllExistingErrorsRecursive(this ObservableValidator target)
        {
            ArgumentNullException.ThrowIfNull(target);
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            return GetAllErrorsRecursive(target, visited, validate: false);
        }
        private static List<ValidationResult> GetAllErrorsRecursive(ObservableValidator target, HashSet<object> visited, bool validate)
        {
            // 防成环
            if (!visited.Add(target))
            {
                return new List<ValidationResult>();
            }

            if (validate)
            {
                target.ValidateAllProperties();
            }

            List<ValidationResult> errors = [.. target.GetErrors()];
            foreach (PropertyInfo property in GetRecursiveProperties(target.GetType()))
            {
                object? value = property.GetValue(target);
                if (value == null) continue;

                if (value is ObservableValidator child)
                {
                    errors.AddRange(GetAllErrorsRecursive(child, visited, validate));
                    continue;
                }

                if (value is IEnumerable enumerable)
                {
                    foreach (object item in enumerable)
                    {
                        if (item is ObservableValidator itemValidator)
                        {
                            errors.AddRange(GetAllErrorsRecursive(itemValidator, visited, validate));
                        }
                    }
                }
            }

            return errors;
        }
        private static PropertyInfo[] GetRecursiveProperties(Type type)
        {
            if (RecursivePropertiesCache.TryGetValue(type, out PropertyInfo[]? cached))
            {
                return cached;
            }

            cached = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !p.GetIndexParameters().Any())
                .Where(p => typeof(ObservableValidator).IsAssignableFrom(p.PropertyType) || (p.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(p.PropertyType)))
                .ToArray();

            RecursivePropertiesCache[type] = cached;
            return cached;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ValidationMetadataAttribute : Attribute
    {
        public string? Method { get; set; }
    }

}
