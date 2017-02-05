using System.Xml.Serialization;

namespace Markify.Services.Roslyn.Fixtures
{
    [XmlRoot("SolutionFixture")]
    public class Solutions
    {
        #region Properties

        [XmlArray("Solutions")]
        [XmlArrayItem("Solution")]
        public SolutionContent[] AllSolution { get; set; }

        #endregion
    }
}