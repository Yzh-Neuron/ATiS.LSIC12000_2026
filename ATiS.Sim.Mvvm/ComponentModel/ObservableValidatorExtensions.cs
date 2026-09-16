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
        public static List<ValidationResult> GetAllErrorsRecursive(this ObservableValidator target)
        {
            List<ValidationResult> errors = [.. target.GetErrors()];

            PropertyInfo[] properties = target.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !p.GetIndexParameters().Any())
                .ToArray();

            foreach (PropertyInfo property in properties)
            {
                Type propType = property.PropertyType;
                object? value = property.GetValue(target);

                if (value != null)
                {
                    if (value is ObservableValidator child)
                    {
                        errors.AddRange(child.GetAllErrorsRecursive());
                        continue;
                    }

                    if (propType != typeof(string) && value is IEnumerable enumerable)
                    {
                        Type? elementType = GetEnumerableElementType(propType);
                        if (elementType == null || !typeof(ObservableValidator).IsAssignableFrom(elementType))
                        {
                            continue;
                        }

                        foreach (object item in enumerable)
                        {
                            if (item is ObservableValidator npcItem)
                            {
                                errors.AddRange(npcItem.GetAllErrorsRecursive());
                            }
                        }
                    }
                }
            }
            return errors;
        }
        private static Type? GetEnumerableElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            Type? ienum = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                ? type
                : type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return ienum?.GetGenericArguments()[0];
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ValidationMetadataAttribute : Attribute
    {
        public string? Method { get; set; }
    }

}
