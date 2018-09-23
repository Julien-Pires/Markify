$coverageFilePath = Resolve-Path -path "TestResults\*\*.coverage"
$coverageFilePath = $coverageFilePath.ToString()

."C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" analyze /output:coverage.xml "$coverageFilePath"

csmacnz.Coveralls --dynamiccodecoverage -i coverage.xml `
    --repoToken $env:COVERALLS_REPO_TOKEN `
    --commitId $env:Build.SourceVersion `
    --commitBranch $env:Build.SourceBranch `
    --commitAuthor $env:Build.RequestedFor `
    --commitEmail $env:Build.RequestedForEmail `
    --commitMessage $env:Build.SourceVersionMessage `
    --jobId $env:Build.BuildId `
    --useRelativePaths -o cov.json