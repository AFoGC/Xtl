using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Xtl
{
    public static class Helper
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is not MemberExpression member)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            if (member.Member is not PropertyInfo propInfo)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));
            }

            Type type = typeof(TSource);
            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));
            }

            return propInfo;
        }

        private static readonly XmlWriterSettings _settings = new XmlWriterSettings() { OmitXmlDeclaration = true};
        private static readonly XmlDocument _innerDoc = new XmlDocument();

        public static string ToXmlValue<T>(T value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            var output = new StringBuilder();
            using (var stringwriter = XmlWriter.Create(output, _settings))
            {
                serializer.Serialize(stringwriter, value);
                _innerDoc.LoadXml(output.ToString());

                if (_innerDoc.DocumentElement != null)
                {
                    return _innerDoc.DocumentElement.InnerXml;
                }
                else
                {
                    throw new XmlException($"Serialization problem. Type:{typeof(T)}. Value{value}");
                }
            }
        }
        
    }
}
