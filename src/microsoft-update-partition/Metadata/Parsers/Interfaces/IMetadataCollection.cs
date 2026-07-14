namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;

/// <summary>
/// Defines a contract for collections of metadata objects that can be created from an XPathNavigator and XmlNamespaceManager.
/// </summary>
/// <typeparam name="T">The type of metadata objects in the collection.</typeparam>
internal interface IMetadataCollection<out T> where T : ICreatable<T> { }