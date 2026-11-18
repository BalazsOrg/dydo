namespace DynaDocs.Tests.Services;

using DynaDocs.Models;
using DynaDocs.Services;

/// <summary>
/// dydo.json is the guard's own configuration: a strict load refuses a malformed file instead of
/// reading it as absent, a save that changes nothing writes nothing, and a save that fails at any
/// boundary leaves the original bytes and no temporary sibling. Each failure is injected, which
/// driving the CLI cannot do.
/// </summary>
[Trait("Job", "trap: DYD-134 (atomic save boundaries: a failed save must keep dydo.json); edge: injected write, flush, close and replace failures")]
public class ConfigServiceTests : IDisposable
{
    private readonly string _testDir;

    public ConfigServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "dydo-config-test-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_testDir);

        File.WriteAllText(Path.Combine(_testDir, "dydo.json"), """
            {
                "version": 1,
                "structure": { "root": "dydo" }
            }
            """);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }

    [Fact]
    public void LoadConfigStrict_DistinguishesMalformedFromMissingConfiguration()
    {
        File.WriteAllText(Path.Combine(_testDir, "dydo.json"), "{ not-json");

        var error = Assert.Throws<InvalidDataException>(
            () => new ConfigService().LoadConfigStrict(_testDir));

        Assert.Contains("dydo.json", error.Message);
        Assert.Contains("Invalid JSON", error.Message);
    }

    [Fact]
    public void SaveConfig_UnchangedConfig_WritesNothing()
    {
        var path = Path.Combine(_testDir, "dydo.json");
        var service = new ConfigService();
        var config = service.LoadConfig(_testDir)!;

        service.SaveConfig(config, path, ConfigSaveOperations.Default with
        {
            CreateNew = _ => throw new InvalidOperationException("an unchanged config was written")
        });
    }

    [Theory]
    [InlineData("{\"testing\":[]}")]
    [InlineData("{\"testing\":{}}")]
    [InlineData("{\"testing\":{\"runner\":[]}}")]
    [InlineData("{\"testing\":{\"runner\":[\"\"]}}")]
    [InlineData("{\"testing\":{\"runner\":[1]}}")]
    [InlineData("{\"testing\":{\"runner\":[\"runner\",\"\\u0000\"]}}")]
    public void LoadConfigStrict_RejectsInvalidTestingRunner(string json)
    {
        File.WriteAllText(Path.Combine(_testDir, "dydo.json"), json);

        var error = Assert.Throws<InvalidDataException>(() => new ConfigService().LoadConfigStrict(_testDir));

        Assert.Contains("dydo.json testing.runner", error.Message);
    }

    [Fact]
    public void LoadConfigStrict_PropagatesAnUnreadableConfiguration()
    {
        var path = Path.Combine(_testDir, "dydo.json");
        using var locked = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        Assert.Throws<IOException>(() => new ConfigService().LoadConfigStrict(_testDir));
    }

    // The six injected boundaries of the atomic save, each spelled here rather than taken from a
    // production global, so one test replaces exactly the boundary it fails.
    private static void SaveWith(
        string path,
        string temporary,
        Action<FileStream, byte[]>? writeAll = null,
        Action<FileStream>? durableFlush = null,
        Action<FileStream>? close = null,
        Action<string, string>? replace = null)
        => new ConfigService().SaveConfig(new DydoConfig { Version = 7 }, path,
            new ConfigSaveOperations(
                _ => temporary,
                candidate => new FileStream(candidate, FileMode.CreateNew, FileAccess.Write, FileShare.None),
                writeAll ?? ((stream, bytes) => stream.Write(bytes)),
                durableFlush ?? (stream => stream.Flush(flushToDisk: true)),
                close ?? (stream => stream.Dispose()),
                replace ?? ((source, target) => File.Move(source, target, overwrite: true))));

    [Fact]
    public void SaveConfig_Success_AtomicallyReplacesAndLeavesNoTemporarySibling()
    {
        var path = Path.Combine(_testDir, "atomic-success.json");
        var temporary = path + ".owned.tmp";
        File.WriteAllBytes(path, "ORIGINAL"u8.ToArray());

        SaveWith(path, temporary);

        Assert.Contains("\"version\": 7", File.ReadAllText(path));
        Assert.False(File.Exists(temporary));
    }

    [Fact]
    public void SaveConfig_PartialTemporaryWriteFailure_PreservesOriginalAndCleansTemporarySibling()
    {
        var path = Path.Combine(_testDir, "partial-write.json");
        var temporary = path + ".owned.tmp";
        var original = "ORIGINAL-PARTIAL-WRITE"u8.ToArray();
        File.WriteAllBytes(path, original);

        var failure = Assert.Throws<IOException>(() => SaveWith(path, temporary, writeAll: (stream, bytes) =>
        {
            stream.Write(bytes.AsSpan(0, 3));
            throw new IOException("injected partial write failure");
        }));

        Assert.Equal("injected partial write failure", failure.Message);
        Assert.Equal(original, File.ReadAllBytes(path));
        Assert.False(File.Exists(temporary));
    }

    [Fact]
    public void SaveConfig_FlushFailure_PreservesOriginalAndCleansTemporarySibling()
    {
        var path = Path.Combine(_testDir, "flush.json");
        var temporary = path + ".owned.tmp";
        var original = "ORIGINAL-FLUSH"u8.ToArray();
        File.WriteAllBytes(path, original);

        var failure = Assert.Throws<IOException>(() => SaveWith(path, temporary,
            durableFlush: _ => throw new IOException("injected durable flush failure")));

        Assert.Equal("injected durable flush failure", failure.Message);
        Assert.Equal(original, File.ReadAllBytes(path));
        Assert.False(File.Exists(temporary));
    }

    [Fact]
    public void SaveConfig_CloseFailure_PreservesOriginalAndCleansTemporarySibling()
    {
        var path = Path.Combine(_testDir, "close.json");
        var temporary = path + ".owned.tmp";
        var original = "ORIGINAL-CLOSE"u8.ToArray();
        File.WriteAllBytes(path, original);

        var failure = Assert.Throws<IOException>(() => SaveWith(path, temporary, close: stream =>
        {
            stream.Dispose();
            throw new IOException("injected close failure");
        }));

        Assert.Equal("injected close failure", failure.Message);
        Assert.Equal(original, File.ReadAllBytes(path));
        Assert.False(File.Exists(temporary));
    }

    [Fact]
    public void SaveConfig_ReplacementFailure_PreservesOriginalAndCleansTemporarySibling()
    {
        var path = Path.Combine(_testDir, "replace.json");
        var temporary = path + ".owned.tmp";
        var original = "ORIGINAL-REPLACE"u8.ToArray();
        File.WriteAllBytes(path, original);

        var failure = Assert.Throws<IOException>(() => SaveWith(path, temporary,
            replace: (_, _) => throw new IOException("injected replacement failure")));

        Assert.Equal("injected replacement failure", failure.Message);
        Assert.Equal(original, File.ReadAllBytes(path));
        Assert.False(File.Exists(temporary));
    }

    [Fact]
    public void SaveConfig_TemporaryNameCollision_PreservesBothFiles()
    {
        var path = Path.Combine(_testDir, "collision.json");
        var temporary = path + ".collision.tmp";
        var original = "ORIGINAL-COLLISION"u8.ToArray();
        var collision = "PREEXISTING-SIBLING"u8.ToArray();
        File.WriteAllBytes(path, original);
        File.WriteAllBytes(temporary, collision);

        var failure = Assert.Throws<IOException>(() => SaveWith(path, temporary));

        Assert.Contains("already exists", failure.Message);
        Assert.Equal(original, File.ReadAllBytes(path));
        Assert.Equal(collision, File.ReadAllBytes(temporary));
    }

    // The two production boundaries the injected tests above cannot reach: where the sibling
    // goes, and how it is opened.
    [Fact]
    public void TemporarySiblingPath_IsAUniqueTmpBesideTheTarget()
    {
        var target = Path.Combine(_testDir, "dydo.json");

        var first = ConfigService.TemporarySiblingPath(target);
        var second = ConfigService.TemporarySiblingPath(target);

        Assert.Equal(_testDir, Path.GetDirectoryName(first));
        Assert.Matches(@"^dydo\.json\.[0-9a-f]{32}\.tmp$", Path.GetFileName(first));
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void CreateNewSibling_RefusesAnExistingFile()
    {
        var sibling = Path.Combine(_testDir, "dydo.json.taken.tmp");
        File.WriteAllBytes(sibling, "PREEXISTING"u8.ToArray());

        Assert.Throws<IOException>(() => ConfigService.CreateNewSibling(sibling));

        Assert.Equal("PREEXISTING"u8.ToArray(), File.ReadAllBytes(sibling));
    }

    [Fact]
    public void CreateNewSibling_OpensWriteOnlyAndExcludesOtherHandles()
    {
        var sibling = Path.Combine(_testDir, "dydo.json.fresh.tmp");

        using var stream = ConfigService.CreateNewSibling(sibling);

        Assert.True(stream.CanWrite);
        Assert.False(stream.CanRead);
        Assert.Throws<IOException>(() => File.OpenRead(sibling));
    }

}