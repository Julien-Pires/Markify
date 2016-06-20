namespace Markify.Core.IDE
{
    public interface ISolutionExplorerFilterProvider
    {
        #region Properties
    
        SolutionExplorerFilter Filters { get; }

        #endregion
    }
}