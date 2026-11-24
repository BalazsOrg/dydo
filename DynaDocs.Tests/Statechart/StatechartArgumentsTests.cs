namespace DynaDocs.Tests.Statechart;

using System.Runtime.InteropServices;
using DynaDocs.Services.Statechart;

public class StatechartArgumentsTests
{
    [Fact]
    [Trait("Job", "trap: a NUL inside a native argument would silently truncate it, so building the vector refuses it")]
    public void NativeArgumentVectorsRejectTruncatingNulBeforeAllocation()
    {
        Assert.Throws<ArgumentException>(() => new PosixStatechartArguments(["before\0after"]));
    }

    [Fact]
    [Trait("Job", "edge: the native argument vector holds non-ASCII and empty arguments as UTF-8, ends with NULL, and reads 0 as its pointer once disposed")]
    public void NativeArgumentVectorsPreserveUtf8AndTerminateWithNull()
    {
        using var arguments = new PosixStatechartArguments(["spaces and é", ""]);
        Assert.Equal("spaces and é", Marshal.PtrToStringUTF8(Marshal.ReadIntPtr(arguments.Pointer)));
        Assert.Equal("", Marshal.PtrToStringUTF8(Marshal.ReadIntPtr(arguments.Pointer, nint.Size)));
        Assert.Equal(0, Marshal.ReadIntPtr(arguments.Pointer, 2 * nint.Size));
        arguments.Dispose();
        Assert.Equal(0, arguments.Pointer);
    }
}
