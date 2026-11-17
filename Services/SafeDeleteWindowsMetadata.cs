namespace DynaDocs.Services;

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static class SafeDeleteWindowsMetadata
{
    internal static string Read(string path)
    {
        using var handle = CreateFile(path, 0x80, 7, IntPtr.Zero, 3, 0x02000000, IntPtr.Zero);
        var info = new byte[52];
        if (handle.IsInvalid || !GetInformation(handle, info) || BitConverter.ToUInt32(info, 40) > 1)
            throw new InvalidDataException($"Windows file identity/hard-link metadata unsupported: {path}");
        CheckStreams(path);
        GetSecurity(path, 7, null, 0, out var length);
        if (length == 0) throw new IOException("Windows security descriptor is unreadable");
        var descriptor = new byte[length];
        if (!GetSecurity(path, 7, descriptor, length, out _)) throw new IOException("Cannot capture Windows permissions");
        return Convert.ToBase64String(descriptor);
    }

    internal static void Apply(string path, string encoded)
    {
        var descriptor = Convert.FromBase64String(encoded);
        if (descriptor.Length < 20) throw new InvalidDataException("Invalid Windows security descriptor");
        var control = BitConverter.ToUInt16(descriptor, 2);
        var flags = (control & 0x1000) != 0 ? 0x80000007u : 0x20000007u;
        if (!SetSecurity(path, flags, descriptor)) throw new IOException("Cannot restore Windows permissions; archive retained");
    }

    // Each native call reads its Win32 error in the call's own statement. Anything that runs between two
    // statements, a coverage visit included, may make P/Invoke calls of its own and replace that error.
    private static void CheckStreams(string path)
    {
        var data = new byte[600];
        var (handle, error) = (FindFirstStream(path, 0, data, 0), Marshal.GetLastPInvokeError());
        if (handle == new IntPtr(-1))
        {
            if (error == 38) return; // ERROR_HANDLE_EOF: no data streams, as for a directory.
            throw new IOException($"Cannot establish Windows stream metadata: Win32 error {error}");
        }
        try
        {
            bool more;
            do
            {
                var name = System.Text.Encoding.Unicode.GetString(data, 8, 592).TrimEnd('\0');
                if (name != "::$DATA") throw new InvalidDataException("Alternate Windows streams are unsupported; target retained");
                (more, error) = (FindNextStream(handle, data), Marshal.GetLastPInvokeError());
            } while (more);
            if (error != 38) throw new IOException($"Incomplete Windows stream metadata: Win32 error {error}");
        }
        finally { FindClose(handle); }
    }

    [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(string path, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);
    [DllImport("kernel32.dll", EntryPoint = "GetFileInformationByHandle", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetInformation(SafeFileHandle handle, [Out] byte[] information);
    [DllImport("advapi32.dll", EntryPoint = "GetFileSecurityW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSecurity(string path, uint flags, [Out] byte[]? descriptor, uint length, out uint needed);
    [DllImport("advapi32.dll", EntryPoint = "SetFileSecurityW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetSecurity(string path, uint flags, byte[] descriptor);
    [DllImport("kernel32.dll", EntryPoint = "FindFirstStreamW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr FindFirstStream(string path, int level, [Out] byte[] data, uint flags);
    [DllImport("kernel32.dll", EntryPoint = "FindNextStreamW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FindNextStream(IntPtr handle, [Out] byte[] data);
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FindClose(IntPtr handle);
}
