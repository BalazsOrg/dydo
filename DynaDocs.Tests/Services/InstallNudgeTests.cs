namespace DynaDocs.Tests.Services;

using DynaDocs.Tests.Integration;

/// <summary>
/// DYD-499: the default library-install warn nudge, at the hook. An install or add meets it once and
/// the identical retry passes; restoring locked dependencies, and everything else, never meets it.
/// A chain after cd is the cd-chain coaching's, so no row starts with cd.
/// </summary>
[Collection("Integration")]
public class InstallNudgeTests : GuardHookTestBase
{
    private static readonly string[] Installs =
    [
        "pnpm add left-pad", "pnpm add -D vitest", "pnpm -C client add x", "pnpm --filter web add x",
        "pnpm -r add x", "pnpm dlx create-vite", "pnpm create vite", "pnpm update", "pnpm up left-pad",
        "pnpm upgrade", "pnpm link ../x", "pnpm install left-pad", "pnpm i left-pad", "npm install left-pad",
        "npm install -D vitest", "npm i left-pad", "npm update", "yarn add left-pad", "bun add left-pad",
        "npx -y cowsay", "npx --yes cowsay", "pnpx -p x y", "uv add requests", "uv remove requests",
        "uv pip install requests", "uv tool install ruff", "uv run --with rich python x.py", "uvx ruff",
        "pipx install black", "pip install x", "pip3 install x", ".venv/bin/pip install x",
        "python -m pip install x", "python3.12 -m pip install x", "py -m pip install x",
        "tools/world/.venv/bin/python -m pip install x", "dotnet add package Newtonsoft.Json",
        "dotnet tool install -g dotnet-ef", "dotnet tool update -g dotnet-ef", "dotnet workload install wasm-tools",
        "dotnet new install Foo.Templates", "apt install x", "apt-get install x", "sudo apt install x",
        "snap install x", "flatpak install x", "cargo install ripgrep", "go install example.com/x@latest",
        "gem install rails", "brew install jq", "env FOO=1 pip install x", "FOO=1 pip install x",
        "git pull | pnpm add x", "PNPM ADD x",
    ];

    private static readonly string[] NoInstalls =
    [
        "pnpm install", "npm install", "pnpm install --frozen-lockfile",
        "pnpm install --frozen-lockfile --dir client", "pnpm install --frozen-lockfile; pnpm test",
        "pnpm install 2>&1 | tail -5", "pnpm install > log.txt", "npm install 2>&1", "npm ci",
        "yarn install --frozen-lockfile", "bun install", "uv sync", "uv sync --frozen", "uv sync --locked",
        "uv run pytest", "dotnet restore", "dotnet tool restore", "pnpm test", "pnpm run build",
        "npm run install-hooks", "pip list", "pip freeze", "npm ls", "pnpm list", "uv pip list", "dotnet build",
        "dotnet test", "dotnet list package", "apt list --installed", "brew list", "cargo build", "go build ./...",
        "git commit -m 'pip install x'", "echo pnpm add x", "grep -r \"pip install\" docs",
    ];

    public static TheoryData<string> InstallCommands => [.. Installs];

    public static TheoryData<string> OtherCommands => [.. NoInstalls];

    [Theory]
    [Trait("Job", "contract: DYD-499 installing a library or tool needs the user's permission (an install or add meets the nudge once, and the identical retry passes)")]
    [MemberData(nameof(InstallCommands))]
    public async Task Install_MeetsTheNudgeOnce_AndTheSameRetryPasses(string command)
    {
        await InitProjectAsync("none");

        AssertRefused("Bash", await ShellAsync("Bash", command));
        AssertAllowed("Bash", await ShellAsync("Bash", command));
    }

    [Theory]
    [Trait("Job", "contract: DYD-499 (a command that restores locked dependencies or installs nothing never meets the library-install nudge)")]
    [MemberData(nameof(OtherCommands))]
    public async Task RestoreOrOtherCommand_NeverMeetsTheNudge(string command)
    {
        await InitProjectAsync("none");

        AssertAllowed("Bash", await ShellAsync("Bash", command));
    }
}
