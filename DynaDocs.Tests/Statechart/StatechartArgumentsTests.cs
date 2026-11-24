namespace DynaDocs.Tests.Statechart;

using System.Runtime.InteropServices;
using DynaDocs.Services.Statechart;

public class StatechartArgumentsTests
{
    [Fact]
    [Trait("Job", "trap: a NUL inside a native argument vector would silently truncate the argument")]
    public void NativeArgumentVectorsRejectTruncatingNulBeforeAllocation()
    {
        Assert.Throws<ArgumentException>(() => new PosixStatechartArguments(["before\0after"]));
    }

    [Fact]
    [Trait("Job", "edge: non-ASCII and empty arguments reach exec intact and NULL-terminated, and the vector is freed")]
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
