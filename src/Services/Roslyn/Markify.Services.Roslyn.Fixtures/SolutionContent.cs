using System.Xml.Serialization;

namespace Markify.Services.Roslyn.Fixtures
{
    [XmlRoot("Solution")]
    public class SolutionContent
    {
        #region Properties

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlArray("Projects")]
        [XmlArrayItem("Project")]
        public ProjectContent[] Projects { get; set; } = new ProjectContent[0];

        #endregion
    }
}