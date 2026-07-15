using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Parsers.Interfaces;

/// <summary>
/// Defines a contract for types that can be created from an XPathNavigator and XmlNamespaceManager.
/// </summary>
/// <typeparam name="T">The type that can be created.</typeparam>
internal interface ICreatable<T>
{
    static abstract string[] ValidParentNodes { get; }
    static abstract string XPathQuery { get; }

	static abstract T Create(XPathNavigator navigator, XmlNamespaceManager manager);
}
