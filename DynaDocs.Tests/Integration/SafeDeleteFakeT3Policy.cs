namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;

internal sealed class SafeDeleteFakeT3Policy(string? refusal, bool routeProofEnabled = false) : ISafeDeleteT3Policy
{
    public string? Refusal(string path) => refusal ?? (routeProofEnabled ? null : "unproven settled T3 route");
}
