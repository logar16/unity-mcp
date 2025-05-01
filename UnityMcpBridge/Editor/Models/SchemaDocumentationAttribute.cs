// Provides documentation for schema generation on fields/properties.
using System;

namespace UnityMcp.Editor.Models
{
    /// <summary>
    /// Provides documentation for schema generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SchemaDocumentationAttribute : Attribute
    {
        /// <summary>
        /// A concise description of the property or field.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Additional notes, usage examples, or implementation details.
        /// Supports multi-line strings.
        /// </summary>
        public string Notes { get; set; }

        public SchemaDocumentationAttribute(string description)
        {
            Description = description;
        }
    }
}
