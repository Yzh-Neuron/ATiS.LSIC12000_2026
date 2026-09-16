using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ATiS.Sim.Mvvm
{
    public class ObservableValidator : ObservableObject, INotifyDataErrorInfo
    {
        private static readonly ConditionalWeakTable<Type, Dictionary<string, ValidationMeta?>> ValidationMetaCache = new();
        private static readonly PropertyChangedEventArgs HasErrorsChangedEventArgs = new(nameof(HasErrors));

        private int _totalErrors;
        private readonly Dictionary<string, List<ValidationResult>> _errors = new();

        [Display(AutoGenerateField = false)]
        public bool HasErrors => _totalErrors > 0;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public ObservableValidator()
        {
            ValidateAllProperties();
        }

        [RequiresUnreferencedCode("The type of the current instance cannot be statically discovered.")]
        protected bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, bool validate, [CallerMemberName] string propertyName = null!)
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            bool propertyChanged = SetProperty(ref field, newValue, propertyName);
            if (propertyChanged && validate)
            {
                ValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        [RequiresUnreferencedCode("The type of the current instance cannot be statically discovered.")]
        private void ValidateProperty(object? value, [CallerMemberName] string propertyName = null!)
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            Type runtimeType = GetType();
            ValidationMeta? validationMeta = null;

            if (ValidationMetaCache.TryGetValue(runtimeType, out var cache))
            {
                if (!cache.TryGetValue(propertyName, out validationMeta))
                {
                    PropertyInfo? prop = runtimeType.GetProperty(propertyName);
                    if (prop != null)
                    {
                        validationMeta = BuildValidationMeta(runtimeType, prop);
                    }
                    cache.TryAdd(propertyName, validationMeta);
                }
            }

            // 初始化当前属性的错误列表
            if (!_errors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors))
            {
                propertyErrors = new List<ValidationResult>();
                _errors.Add(propertyName, propertyErrors);
            }

            bool hadOldErrors = propertyErrors.Count > 0;
            propertyErrors.Clear();

            // 执行校验，收集新错误
            bool hasNewError = false;
            if (validationMeta is { ValidatorFunc: not null })
            {
                ValidationResult result = validationMeta.ValidatorFunc.Invoke(this, value);
                if (result!=null &&!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    propertyErrors.Add(result);
                    hasNewError = true;
                }
            }
            if (hadOldErrors) _totalErrors--;
            if (hasNewError) _totalErrors++;

            bool stateChanged = (hadOldErrors && !hasNewError) || (!hadOldErrors && hasNewError);
            if (stateChanged && _totalErrors is 0 or 1)
            {
                OnPropertyChanged(HasErrorsChangedEventArgs);
            }
            if (hadOldErrors || hasNewError)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
        private void ValidateAllProperties()
        {
            Type runtimeType = GetType();
            bool isFirstCreate = false;

            var cache = ValidationMetaCache.GetValue(runtimeType, _ =>
            {
                isFirstCreate = true;
                return new Dictionary<string, ValidationMeta?>();
            });

            if (isFirstCreate)
            {
                IEnumerable<PropertyInfo> validateProps = runtimeType
                     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => !p.GetIndexParameters().Any() && p.IsDefined(typeof(ValidationMetadataAttribute), false));

                foreach (PropertyInfo prop in validateProps)
                {
                    if (!cache.ContainsKey(prop.Name))
                    {
                        ValidationMeta? meta = BuildValidationMeta(runtimeType, prop);
                        cache.TryAdd(prop.Name, meta);
                    }
                }
            }

            foreach (var kv in cache)
            {
                if (kv.Value == null) continue;
                object? value = kv.Value.Property.GetValue(this);
                ValidateProperty(value, kv.Key);
            }
        }
        private ValidationMeta? BuildValidationMeta(Type runtimeType, PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<ValidationMetadataAttribute>();
            if (attr == null || string.IsNullOrWhiteSpace(attr.Method))
            {
                return null;
            }

            MethodInfo? method = runtimeType.GetMethod(
                attr.Method,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                binder: null,
                types: [property.PropertyType],
                modifiers: null);

            if (method == null)
            {
                return null;
            }

            // 表达式树：编译强类型校验委托
            ParameterExpression instanceParam = Expression.Parameter(typeof(ObservableValidator), "instance");
            ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");

            Expression instanceExpr = method.IsStatic
                ? Expression.Default(method.DeclaringType!)
                : Expression.Convert(instanceParam, method.DeclaringType!);

            Type targetType = property.PropertyType;
            Expression argValue;
            bool isNullableValueType = Nullable.GetUnderlyingType(targetType) != null;

            if (targetType.IsValueType && !isNullableValueType)
            {
                BinaryExpression isNull = Expression.Equal(valueParam, Expression.Constant(null, typeof(object)));
                Expression defaultValue = Expression.Default(targetType);
                UnaryExpression convertValue = Expression.Convert(valueParam, targetType);
                argValue = Expression.Condition(isNull, defaultValue, convertValue);
            }
            else
            {
                argValue = Expression.Convert(valueParam, targetType);
            }

            MethodCallExpression callExpr = Expression.Call(instanceExpr, method, argValue);
            UnaryExpression resultCast = Expression.Convert(callExpr, typeof(ValidationResult));

            Func<ObservableValidator, object?, ValidationResult> validatorFunc = Expression
                .Lambda<Func<ObservableValidator, object?, ValidationResult>>(resultCast, instanceParam, valueParam)
                .Compile();

            return new ValidationMeta(validatorFunc, attr, property);
        }
        IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => GetErrors(propertyName);
        public IEnumerable<ValidationResult> GetErrors(string? propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                IEnumerable<ValidationResult> GetAllErrors()
                {
                    return _errors.Values.SelectMany(static errors => errors);
                }
                return GetAllErrors();
            }

            return _errors.TryGetValue(propertyName!, out List<ValidationResult>? errors)
                ? errors
                : Array.Empty<ValidationResult>();
        }
        private sealed class ValidationMeta
        {
            public PropertyInfo Property { get; }
            public ValidationMetadataAttribute Attr { get; }
            public Func<ObservableValidator, object?, ValidationResult> ValidatorFunc { get; }
            public ValidationMeta(Func<ObservableValidator, object?, ValidationResult> validatorFunc, ValidationMetadataAttribute attr, PropertyInfo property)
            {
                ValidatorFunc = validatorFunc;
                Attr = attr;
                Property = property;
            }
        }
    }
}