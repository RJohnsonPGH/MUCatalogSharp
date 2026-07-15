using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata.Drivers;

public sealed record FeatureScore(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<FeatureScore>, IMetadataCollection<FeatureScore>, IComparable
{
	static string[] ICreatable<FeatureScore>.ValidParentNodes => ["drv:WindowsDriverMetaData"];
	static string ICreatable<FeatureScore>.XPathQuery => "drv:FeatureScore";
	static FeatureScore ICreatable<FeatureScore>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    public string OperatingSystem => _operatingSystem.Value;
    private readonly Lazy<string> _operatingSystem = new(() => 
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@OperatingSystem"));

	public int Score => _score.Value;
	private readonly Lazy<int> _score = new(() =>
		UpdateParser.CreateProperty<ParsableHexInt>(Navigator, Manager, "@FeatureScore"));

    /// <summary>
    /// Compare two driver feature scores
    /// <para>
    /// A smaller feature score is better; if sorting feature scores, take the smaller value as the better driver
    /// </para>
    /// </summary>
    /// <param name="obj">Other object</param>
    /// <returns>-1 if other feature score is lower (better), 0 if equal and 1 if higher (worse)</returns>
    public int CompareTo(object? obj)
    {
        return obj switch
        {
            null => 1,
            FeatureScore other => this.Score.CompareTo(other.Score),
            _ => -1,
        };
    }
}
