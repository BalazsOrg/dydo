namespace DynaDocs.Services;

internal interface ISafeDeleteT3Policy
{
    // Null means independently established eligibility, never merely an empty thread-list result.
    string? Refusal(string path);
}
