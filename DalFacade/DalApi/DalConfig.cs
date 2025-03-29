namespace DalApi;
using System.Xml.Linq;
/// <summary>
/// Static class for handling DAL configuration.
/// Reads the configuration from an XML file and initializes DAL settings.
/// </summary>
static class DalConfig
{
    /// <summary>
    /// Internal record representing a DAL implementation.
    /// </summary>
    internal record DalImplementation
    (
        string Package,   
        string Namespace, 
        string Class   
    );
    /// <summary>
    /// Name of the currently selected DAL implementation.
    /// </summary>
    internal static string s_dalName;

    /// <summary>
    /// Dictionary mapping DAL package names to their implementations.
    /// </summary>
    internal static Dictionary<string, DalImplementation> s_dalPackages;

    /// <summary>
    /// Static constructor to load DAL configuration from XML file.
    /// </summary>
    static DalConfig()
    {
        XElement dalConfig = XElement.Load(@"..\xml\dal-config.xml") ??
        throw new DalConfigException("dal-config.xml file is not found");

        s_dalName = dalConfig.Element("dal")?.Value ?? throw new DalConfigException("<dal> element is missing");

        var packages = dalConfig.Element("dal-packages")?.Elements() ??
        throw new DalConfigException("<dal-packages> element is missing");
        s_dalPackages = (
                         from item in packages
                         let pkg = item.Value
                         let ns = item.Attribute("namespace")?.Value ?? "Dal"
                         let cls = item.Attribute("class")?.Value ?? pkg
                         select (item.Name, new DalImplementation(pkg, ns, cls))
                        ).ToDictionary(p => "" + p.Name, p => p.Item2);
    }
}
/// <summary>
/// Custom exception class for DAL configuration errors.
/// </summary>
[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}
