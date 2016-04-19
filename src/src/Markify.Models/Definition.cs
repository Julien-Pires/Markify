namespace Markify.Models
{
    public abstract class Definition
    {
        #region Properties

        public string Fullname { get; }

        public string MemberName { get; set; }

        public string Summary { get; set; }

        #endregion

        #region Constructors

        protected Definition(string name)
        {
            Fullname = MemberName = name;
        }

        #endregion
    }
}