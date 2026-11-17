namespace DynaDocs.Models;

internal sealed record SafeDeleteFile(string Path, string Kind, string Hash, long Length, int Mode, int Attributes, string? LinkTarget, string? WindowsSecurity = null);
