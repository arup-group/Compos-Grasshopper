$relevantPath = "ComposGH\bin\x64\Debug\net48"
$absolutePath = Resolve-Path $relevantPath

$destinationDir = "$env:APPDATA\Grasshopper\Libraries"

rm "$destinationDir\ComposGhTests.ghlink"
