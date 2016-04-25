using System.Collections.Generic;

namespace Markify.Processors.Roslyn.Models
{
    public class Fullname
    {
        #region Properties

        public IEnumerable<string> Parts { get; }

        #endregion

        #region Constructors

        public Fullname(IEnumerable<string> parts)
        {
            Parts = parts;
        }

        #endregion

        #region Overrides

        public static implicit operator string(Fullname fullname) => fullname.ToString();

        public override string ToString() => string.Join(".", Parts);

        #endregion
    }
}