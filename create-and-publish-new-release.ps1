$isClean = -not (git status --porcelain)

if ($isClean) {
    Write-Host "Repo is clean"
    bump
    git add .
    git commit -m "bump version"
    nuke publish
} else {
    Write-Host "Repo is dirty"
}
