namespace DynaDocs.SkillBuild;

/// <summary>
/// The Verify answer of a truth-checked claim (DYD-1295 1ffbc4c8, 317be5f7): what the running system
/// showed, never what a decision record said. Exactly one of the five; a claim nobody could verify is
/// <see cref="Open"/>, and its line is withheld from the skill and listed for review.
/// </summary>
public sealed record Evidence
{
    private Evidence(EvidenceKind kind, string text)
    {
        Kind = kind;
        Text = text;
    }

    public EvidenceKind Kind { get; }

    public string Text { get; }

    /// <summary>A command as it is typed. A <c>dydo …</c> command is checked against the CLI's own command tree.</summary>
    public static Evidence Command(string command) => new(EvidenceKind.Command, command);

    /// <summary>A tool's schema: the tool and the field that shows it, such as <c>delegate_task: isolation</c>.</summary>
    public static Evidence Tool(string schema) => new(EvidenceKind.Tool, schema);

    /// <summary>A path in this repository, from its root; the run fails when it does not exist.</summary>
    public static Evidence Code(string path) => new(EvidenceKind.Code, path);

    /// <summary>A run that showed it: what was run, and what it showed.</summary>
    public static Evidence Run(string what) => new(EvidenceKind.Run, what);

    /// <summary>Not verified yet, and why. The claim's line is not rendered.</summary>
    public static Evidence Open(string why) => new(EvidenceKind.Open, why);
}
