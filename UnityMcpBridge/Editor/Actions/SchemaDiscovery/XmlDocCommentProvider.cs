using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace UnityMcp.Editor.Actions
{
    /// <summary>
    /// Provides access to XML documentation comments for types and members.
    /// </summary>
    internal class XmlDocCommentProvider
    {
        private readonly Dictionary<string, XElement> _memberElements;

        private XmlDocCommentProvider(Dictionary<string, XElement> memberElements)
        {
            _memberElements = memberElements;
        }

        /// <summary>
        /// Try to load XML documentation from the given file path.
        /// </summary>
        public static XmlDocCommentProvider TryLoad(string xmlPath)
        {
            if (!File.Exists(xmlPath))
                return new XmlDocCommentProvider(new Dictionary<string, XElement>());

            try
            {
                var doc = XDocument.Load(xmlPath);
                var members = doc.Root.Element("members").Elements("member");
                var dict = new Dictionary<string, XElement>();
                foreach (var m in members)
                {
                    var name = m.Attribute("name")?.Value;
                    if (!string.IsNullOrEmpty(name))
                        dict[name] = m;
                }
                return new XmlDocCommentProvider(dict);
            }
            catch
            {
                return new XmlDocCommentProvider(new Dictionary<string, XElement>());
            }
        }

        /// <summary>
        /// Gets the XML doc summary for a type or member.
        /// </summary>
        public string GetSummary(MemberInfo member)
        {
            var key = GetMemberElementName(member);
            if (key != null && _memberElements.TryGetValue(key, out var elem))
                return elem.Element("summary")?.Value?.Trim();
            return null;
        }

        /// <summary>
        /// Gets the XML doc remarks for a type or member.
        /// </summary>
        public string GetRemarks(MemberInfo member)
        {
            var key = GetMemberElementName(member);
            if (key != null && _memberElements.TryGetValue(key, out var elem))
                return elem.Element("remarks")?.Value?.Trim();
            return null;
        }

        /// <summary>
        /// Gets the XML doc summary for a type.
        /// </summary>
        public string GetTypeSummary(Type type)
        {
            var key = GetTypeElementName(type);
            if (key != null && _memberElements.TryGetValue(key, out var elem))
                return elem.Element("summary")?.Value?.Trim();
            return null;
        }

        private static string GetTypeElementName(Type type)
        {
            // "T:Namespace.TypeName"
            return $"T:{type.FullName?.Replace('+', '.')}";
        }

        private static string GetMemberElementName(MemberInfo member)
        {
            if (member is Type t)
                return GetTypeElementName(t);

            if (member is PropertyInfo prop)
                return $"P:{prop.DeclaringType.FullName?.Replace('+', '.')}.{prop.Name}";
            if (member is FieldInfo field)
                return $"F:{field.DeclaringType.FullName?.Replace('+', '.')}.{field.Name}";
            if (member is MethodInfo method)
                return $"M:{method.DeclaringType.FullName?.Replace('+', '.')}.{method.Name}";
            return null;
        }
    }
}
