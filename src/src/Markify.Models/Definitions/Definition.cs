namespace Markify.Models.Definitions
{
    public abstract class Definition
    {
        #region Properties

        public string Fullname { get; }

        public string Name { get; }

        #endregion

        #region Constructors

        protected Definition(string name)
        {
            Fullname = Name = name;
        }

        #endregion
    }
}