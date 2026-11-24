namespace DynaDocs.Tests.Services;

using System.Text;
using DynaDocs.Services;

/// <summary>
/// What `dydo init` writes to a shell profile, observed as the profile's bytes: one sourcing block
/// per shell, written once, after the profile's own bytes in its own encoding. A PowerShell block
/// in the old exact form is repaired in place; a block the user changed is never touched.
/// </summary>
public sealed class ShellCompletionInstallerTests : IDisposable
{
    private const string Marker = "# dydo shell completions";
    private const string Legacy = "dydo completions powershell | Invoke-Expression";
    private const string Corrected = "dydo completions powershell | Out-String | Invoke-Expression";

    private readonly string _dir = Path.Combine(Path.GetTempPath(), "dydo-profile-" + Guid.NewGuid().ToString("N")[..8]);

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (appends a dydo completions line to the detected profile, once)")]
    [InlineData("bash", "eval \"$(dydo completions bash)\"")]
    [InlineData("zsh", "eval \"$(dydo completions zsh)\"")]
    [InlineData("powershell", Corrected)]
    public void Install_NewProfile_WritesTheBlockOnce(string shell, string sourcingLine)
    {
        var profile = Path.Combine(_dir, "nested", "profile");

        ShellCompletionInstaller.InstallToProfile(shell, profile);
        var first = File.ReadAllBytes(profile);
        ShellCompletionInstaller.InstallToProfile(shell, profile);

        Assert.Equal(Encoding.UTF8.GetBytes($"\n{Marker}\n{sourcingLine}\n"), first);
        Assert.Equal(first, File.ReadAllBytes(profile));
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (appends a dydo completions line to the detected profile); trap: profile bytes and encoding must survive")]
    [InlineData("bash", "utf8")]
    [InlineData("powershell", "utf8-bom")]
    [InlineData("powershell", "utf16-le")]
    [InlineData("powershell", "utf32-be")]
    public void Install_ExistingProfile_AppendsAfterItsBytesInItsEncoding(string shell, string encodingName)
    {
        var encoding = ProfileEncoding(encodingName);
        var profile = Path.Combine(_dir, "profile");
        var original = Encode(encoding, "# árvíztűrő 日本語\n");
        Directory.CreateDirectory(_dir);
        File.WriteAllBytes(profile, original);

        ShellCompletionInstaller.InstallToProfile(shell, profile);

        var line = shell == "bash" ? "eval \"$(dydo completions bash)\"" : Corrected;
        Assert.Equal(original.Concat(encoding.GetBytes($"\n{Marker}\n{line}\n")), File.ReadAllBytes(profile));
    }

    [Theory]
    [Trait("Job", "trap: profile bytes and encoding must survive (an old-form PowerShell block is repaired in place, every other byte kept, and a second run changes nothing)")]
    [InlineData("utf8", "\n")]
    [InlineData("utf16-be", "\r\n")]
    public void Install_LegacyPowerShellBlock_IsRepairedInPlaceKeepingEveryOtherByte(string encodingName, string newline)
    {
        var encoding = ProfileEncoding(encodingName);
        var profile = Path.Combine(_dir, "profile.ps1");
        const string custom = "\n# 日本語\n# dydo shell completions suffix\n" + Legacy + "\n";
        Directory.CreateDirectory(_dir);
        File.WriteAllBytes(profile, Encode(encoding, $"{Marker}{newline}{Legacy}{custom}"));

        ShellCompletionInstaller.InstallToProfile("powershell", profile);
        ShellCompletionInstaller.InstallToProfile("powershell", profile);

        Assert.Equal(Encode(encoding, $"{Marker}{newline}{Corrected}{custom}"), File.ReadAllBytes(profile));
    }

    [Theory]
    [Trait("Job", "trap: profile bytes and encoding must survive (a PowerShell block the user changed, or one already correct, is left byte-identical)")]
    [InlineData(Marker + "\n " + Legacy + "\n")]
    [InlineData(Marker + "\n" + Legacy + " # custom\n")]
    [InlineData(Marker + "\nWrite-Output 'custom'\n" + Legacy + "\n")]
    [InlineData(Marker + "\n" + Corrected + "\n")]
    public void Install_CustomizedPowerShellBlock_IsLeftByteIdentical(string content)
    {
        var profile = Path.Combine(_dir, "profile.ps1");
        var original = Encode(new UnicodeEncoding(false, true), content);
        Directory.CreateDirectory(_dir);
        File.WriteAllBytes(profile, original);

        ShellCompletionInstaller.InstallToProfile("powershell", profile);

        Assert.Equal(original, File.ReadAllBytes(profile));
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (the completions line goes to a profile only when a shell is detected: fish writes no profile)")]
    public void Install_UnknownShell_WritesNothing()
    {
        var profile = Path.Combine(_dir, ".fishrc");

        ShellCompletionInstaller.InstallToProfile("fish", profile);

        Assert.False(File.Exists(profile));
    }

    private static Encoding ProfileEncoding(string name) => name switch
    {
        "utf8" => new UTF8Encoding(false),
        "utf8-bom" => new UTF8Encoding(true),
        "utf16-le" => new UnicodeEncoding(false, true),
        "utf16-be" => new UnicodeEncoding(true, true),
        "utf32-be" => new UTF32Encoding(true, true),
        _ => throw new ArgumentOutOfRangeException(nameof(name))
    };

    private static byte[] Encode(Encoding encoding, string content) =>
        encoding.GetPreamble().Concat(encoding.GetBytes(content)).ToArray();

    public void Dispose() => TestDirectory.Delete(_dir);
}
