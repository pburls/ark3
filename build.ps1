if (!$env:APPVEYOR_BUILD_NUMBER) {
    $revision = '1'
}
else {
    $revision = $env:APPVEYOR_BUILD_NUMBER
}
$revision = "ci-{0:D4}" -f [convert]::ToInt32($revision, 10)
Write-Host "Running build as $revision"

$project_file = '.\src\Ark3\Ark3.csproj'
$test_file = '.\test\Ark3.Test\Ark3.Test.csproj'

Write-Host "Restoring solution nuget dependencies"
dotnet restore

Write-Host "Build solution"
dotnet build --configuration Release

Write-Host "Running test project $test_file"
dotnet test $test_file --configuration Release

if($env:APPVEYOR_REPO_BRANCH -ne "master") {
    Write-Host "Packaging $project_file with version suffix $revision"
    dotnet pack $project_file --configuration Release --version-suffix=$revision
}
else {
    Write-Host "Packaging $project_file without version suffix for master branch"
    dotnet pack $project_file --configuration Release
}