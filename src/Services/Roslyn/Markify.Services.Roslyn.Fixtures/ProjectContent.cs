using System.Xml.Serialization;

namespace Markify.Services.Roslyn.Fixtures
{
    [XmlRoot("Project")]
    public class ProjectContent
    {
        #region Properties

        [XmlArray("Files")]
        [XmlArrayItem("Uri")]
        public string[] Files { get; set; } = new string[0];

        [XmlElement("Count")]
        public int Count { get; set; }

        #endregion
    }
}