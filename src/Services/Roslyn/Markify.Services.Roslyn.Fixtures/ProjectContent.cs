using System.Xml.Serialization;

namespace Markify.Services.Roslyn.Fixtures
{
    [XmlRoot("ProjectFixtures")]
    public class Projects
    {
        #region Properties

        [XmlArray("Projects")]
        [XmlArrayItem("Project")]
        public ProjectContent[] All { get; set; }

        #endregion
    }

    [XmlRoot("Project")]
    public class ProjectContent
    {
        #region Properties

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlArray("Files")]
        [XmlArrayItem("Uri")]
        public string[] Files { get; set; } = new string[0];

        [XmlElement("Count")]
        public int Count { get; set; }

        #endregion
    }
}