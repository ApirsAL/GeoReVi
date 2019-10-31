using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GeoReVi
{
    /// <summary>
    /// A helper for expressions
    /// 
    /// General information:
    /// An expression is a sequence of operands and operators that can be evaluated to a single value or method or object or namespace. It can consist of literal values, method invocations, names of variables, names of methods, method parameters or types. Expression can vary from very simple to very complex. The following is an example of Expression: (n<5) && (n>6)
    /// </summary>
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Compiles an expression and gets the functions return value
        /// </summary>
        /// <typeparam name="T">The type of return value</typeparam>
        /// <param name="lambda">The expression to compile</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this Expression<Func<T>> lambda)
        {
            return lambda.Compile().Invoke();
        }

        /// <summary>
        /// Sets the underlying properties value to the given value, from an expression that contains the property
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="lambda">Expression</param>
        /// <param name="value">the value to set the property to</param>
        public static void SetPropertyValue<T>(this Expression<Func<T>> lambda, T value)
        {
            //Converts a lambda () => some.Property, to some.Property
            var expression = (lambda as LambdaExpression).Body as MemberExpression;

            //Get the property information so we can set it
            var propertyInfo = (PropertyInfo)expression.Member;

            var target = Expression.Lambda(expression.Expression).Compile().DynamicInvoke();

            //Set the property value
            propertyInfo.SetValue(target, value);
        }
    }
}
