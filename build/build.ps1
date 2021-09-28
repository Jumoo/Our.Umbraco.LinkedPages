param($version, $suffix, $env='release', [switch]$push=$false)

$fullVersion = $version  #  -join($version, '-', $suffix)
$outFolder = ".\$fullVersion"
$packageProj = "..\src\Our.Umbraco.LinkedPages\Our.Umbraco.LinkedPages.csproj"

dotnet pack $packageProj -c $env -o $outFolder /p:ContinuousIntegrationBuild=true,version=$fullVersion

if ($push) {
    .\nuget.exe push "$outFolder\*.nupkg" -ApiKey AzureDevOps -src https://pkgs.dev.azure.com/jumoo/Public/_packaging/nightly/nuget/v3/index.json
}