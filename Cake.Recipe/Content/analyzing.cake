///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
BuildParameters.Tasks.DupFinderTask = Task("DupFinder")
    .WithCriteria(() => BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping due to not running on Windows")
    .WithCriteria(() => BuildParameters.ShouldRunDupFinder, "Skipping because DupFinder has been disabled")
    .Does(() => RequireTool(ToolSettings.ReSharperTools, () => {
        var settings = new DupFinderSettings() {
            ShowStats = true,
            ShowText = true,
            OutputFile = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml"),
            ExcludeCodeRegionsByNameSubstring = new string [] { "DupFinder Exclusion" },
            ThrowExceptionOnFindingDuplicates = ToolSettings.DupFinderThrowExceptionOnFindingDuplicates ?? true
        };

        if (ToolSettings.DupFinderExcludePattern != null)
        {
            settings.ExcludePattern = ToolSettings.DupFinderExcludePattern;
        }

        if (ToolSettings.DupFinderExcludeFilesByStartingCommentSubstring != null)
        {
            settings.ExcludeFilesByStartingCommentSubstring = ToolSettings.DupFinderExcludeFilesByStartingCommentSubstring;
        }

        if (ToolSettings.DupFinderDiscardCost != null)
        {
            settings.DiscardCost = ToolSettings.DupFinderDiscardCost.Value;
        }

        DupFinder(BuildParameters.SolutionFilePath, settings);
    })
);

BuildParameters.Tasks.AnalyzeTask = Task("Analyze")
    .IsDependentOn("DupFinder");
