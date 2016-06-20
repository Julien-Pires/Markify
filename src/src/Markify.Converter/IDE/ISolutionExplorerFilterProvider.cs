namespace Markify.Core.IDE
{
    public interface ISolutionExplorerFilterProvider
    {
        #region Methods

        SolutionExplorerFilter GetFilterRules();

        #endregion
    }
}