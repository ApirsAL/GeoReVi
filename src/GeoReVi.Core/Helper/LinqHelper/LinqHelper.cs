using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GeoReVi
{
    /// <summary>
    /// Defines a filter from which a expression will be built.
    /// </summary>
    public interface IFilter<TClass> where TClass : class
    {
        /// <summary>
        /// Group of statements that compose this filter.
        /// </summary>
        List<IFilterStatement> Statements { get; set; }
        /// <summary>
        /// Builds a LINQ expression based upon the statements included in this filter.
        /// </summary>
        /// <returns></returns>
        Expression<Func<TClass, bool>> BuildExpression();
    }

    /// <summary>
    /// Defines how a property should be filtered.
    /// </summary>
    public interface IFilterStatement
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        string PropertyName { get; set; }
        /// <summary>
        /// Express the interaction between the property and the constant value 
        /// defined in this filter statement.
        /// </summary>
        Operation Operation { get; set; }
        /// <summary>
        /// Constant value that will interact with the property defined in this filter statement.
        /// </summary>
        object Value { get; set; }
    }

    public enum Operation
    {
        EqualTo,
        Contains,
        StartsWith,
        EndsWith,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }

    public static class LinqHelper
    {
        public static Func<T, T> DynamicSelectGenerator<T>(string Fields = "")
        {
            string[] EntityFields;
            if (Fields == "")
                // get Properties of the T
                EntityFields = typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToArray();
            else
                EntityFields = Fields.Split(',');

            // input parameter "o"
            var xParameter = Expression.Parameter(typeof(T), "o");

            // new statement "new Data()"
            var xNew = Expression.New(typeof(T));

            // create initializers
            var bindings = EntityFields.Select(o => o.Trim())
                .Select(o =>
                {

                // property "Field1"
                var mi = typeof(T).GetProperty(o);

                // original value "o.Field1"
                var xOriginal = Expression.Property(xParameter, mi);

                // set value "Field1 = o.Field1"
                return Expression.Bind(mi, xOriginal);
                }
            );

            // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(xNew, bindings);

            // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

            // compile to Func<Data, Data>
            return lambda.Compile();
        }
    }
}
