using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PDNUtils.Help
{
    public static class ReflectionHelper
    {
        private static readonly Type[] EmptyTypesArray = { };

        public static object[] GetConstantValues<T>() where T : class
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var xx = from f in fields where (f.IsLiteral && !f.IsInitOnly) select f.GetValue(null);
            return xx.ToArray();
        }

        public static bool CompareInstanceProperties<T>(T obj1, T obj2, IEnumerable<string> ignoredNames, out IEnumerable<PropertyInfo> notEqualProperties) where T : class
        {
            if (obj1 == null || obj2 == null)
            {
                throw new ArgumentNullException("both parameters must be not null", (Exception)null);
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            notEqualProperties = properties.Where(
                p =>
                {
                    if (ignoredNames != null && ignoredNames.Contains(p.Name))
                    {
                        return false;
                    }
                    else
                    {
                        return !PropertiesCompare(obj1, obj2, p);
                    }
                }
                );
            return notEqualProperties.Count() == 0;
        }

        private static bool PropertiesCompare(object obj1, object obj2, PropertyInfo propertyInfo)
        {
            var p1 = GetPropertyValue(obj1, propertyInfo);
            var p2 = GetPropertyValue(obj2, propertyInfo);
            if (p1 == null && p2 == null)
            {
                return true;
            }
            else if (p1 == null && p2 != null)
            {
                return false;
            }
            else if (p1 != null && p2 == null)
            {
                return false;
            }
            else
            {
                return p1.Equals(p2);
            }
        }

        private static object GetPropertyValue<T>(T obj, PropertyInfo p)
        {
            return p.GetValue(obj, BindingFlags.GetProperty, null, null, null);
        }

        public static bool HasToString(object o)
        {
            if (o == null) return false;
            var oType = o.GetType();
            var toStringMethodInfo = oType.GetMethod("ToString", BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.Public, null, EmptyTypesArray, null);
            if (toStringMethodInfo == null) return false;
            Type toStringType = toStringMethodInfo.DeclaringType;
            return oType == toStringType;
        }
    }

}