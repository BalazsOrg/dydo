param([string]$JavaHome = '/usr/lib/jvm/temurin-21-jdk-amd64')

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

# runner-images ubuntu24/20261122.23 (710e8ff2) lists Java 21.0.13+2 on ubuntu-24.04: Temurin
# jdk-21.0.13.1+2, its readme dropping the fourth component. Fail on image drift;
# updating this CI-only pin requires checking the runner metadata, never installing a JDK.
$java = Join-Path $JavaHome 'bin/java'
if (-not (Test-Path $java -PathType Leaf)) { throw 'The runner-provided JDK is missing.' }
$version = & $java --version
if ($LASTEXITCODE -ne 0) { throw 'The runner JDK version check failed.' }
if ($version -cnotcontains 'OpenJDK Runtime Environment Temurin-21.0.13.1+2 (build 21.0.13.1+2-LTS)') {
  throw 'The runner JDK must be Temurin 21.0.13.1+2; review the hosted image and update the CI pin.'
}
$modules = & $java --list-modules
if ($LASTEXITCODE -ne 0 -or $modules -cnotcontains 'jdk.compiler@21.0.13.1') {
  throw 'The runner JDK must provide jdk.compiler@21.0.13.1.'
}
Write-Output $version

$lock = Get-Content Resources/Statechart/dependency-lock.json -Raw | ConvertFrom-Json
$root = New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP 'dydo-statechart-oracle')
$jar = Join-Path $root 'tla2tools.jar'
Invoke-WebRequest -Uri $lock.tlc.url -OutFile $jar
if ((Get-FileHash $jar -Algorithm SHA256).Hash -ne $lock.tlc.sha256) { throw 'TLC does not match its pinned SHA-256.' }
"tla2tools=$jar" >> $env:GITHUB_OUTPUT
"java=$java" >> $env:GITHUB_OUTPUT
