namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Implement kind, writing docs (DYD-1372; spec K31, "separate code and
/// docs resources", and D2 to D5). Its facts are the legacy implementer's docs recipe, each checked
/// against the CLI's own doc checks; a project's conventions are named by the doc that holds them.
/// </summary>
public static class CaptainImplementDocs
{
    private const string Legacy = "skills/roles/crew/implementer/resources/docs.md";
    private const string Conventions = "dydo/reference/writing-docs.md";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Witness = new("Every sentence of a doc is a claim that needs a witness in the repository: code, configuration or governing intent.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 1,
            "Every sentence you write is a claim that needs a witness in the repository: code, configuration, or governing intent such as the spec, a decision record or a ticket. " +
            "Where there is none, return the gap instead of the sentence.")
    ])
    {
        Spec = ["D2", "K32"],
        Truth = new(Evidence.Run("This skill is written so: each of its lines carries what its writer checked, and a line nothing could check is withheld and listed (`dydo/reference/claim-review/ticket-captain-open.md`)."),
            Layer: "`dydo check` reads a doc's frontmatter, names and links, and never whether a sentence in it is true.",
            Reason: "A plausible sentence with nothing behind it is believed by every agent that reads it afterwards.")
    };

    public static readonly Claim BuiltFile = new("A built file changes only in its source.", Me, [])
    {
        Spec = ["K29"],
        Truth = new(Evidence.Code("tools/SkillBuild/BuiltHeader.cs"),
            Layer: "A built file opens in an editor like any other, and its next build silently undoes the edit.",
            Reason: "The header names the source, so a change made there is still standing after the next run.")
    };

    public static readonly Claim OneHome = new("Each meaning has one home, and everything else links to it.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 2,
            "Write each meaning in one home and link to it from everywhere else; change a built file only in the source its header names.", () => [BuiltFile])
    ])
    {
        Spec = ["K29"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "Nothing compares two docs for saying the same thing in different words.",
            Reason: "Two copies drift apart, and a reader cannot tell which of them still holds.")
    };

    public static readonly Claim Agents = new("A skill, its resources or an entry file is written with writing-for-agents.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 3, "Write a skill, its resources or an entry file with /writing-for-agents.")])
    {
        Spec = ["K14", "K31"],
        Truth = new(Evidence.Code("skills/productivity/writing-for-agents/SKILL.md"),
            Layer: "A brief names the skills its dispatcher foresaw, and a writer may meet a skill file only in the middle of its change.",
            Reason: "A text an agent acts on fails in ways ordinary prose never does, and that skill names them.")
    };

    public static readonly Claim Invited = new("A new descriptive doc is written only when invited, and only if its subject is solid, important and expensive to rebuild from code.", Me, [])
    {
        Spec = ["D3", "D4"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "Nothing stops an agent from adding a doc beside the code it has just written.",
            Reason: "Each uninvited doc is one more text to keep true, about code that will move.")
    };

    public static readonly Claim Descriptive = new("A descriptive doc explains and never authorises: on what the code does the code wins, and a reason counts only when it links a decision record or a principle.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 4,
            "A descriptive doc explains and never authorises: on what the code does the code wins, and a reason counts only when it links a decision record or a principle. " +
            "Write a new one only when invited, and only if its subject is solid, important and expensive to rebuild from code.",
            () => [Invited])
    ])
    {
        Spec = ["D2"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A doc and the code it describes can disagree, and nothing in either says which one to trust.",
            Reason: "A doc treated as authority outlives the code it described and steers work toward what is gone.")
    };

    public static readonly Claim Explains = new("When a change touches the paths a doc's explains declares, its writer updates the doc or states that it holds.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 5,
            "When `dydo check` flags a doc whose `explains:` paths your change touches, update the doc or state in your return that it still holds.")
    ])
    {
        Spec = ["D5"],
        Truth = new(Evidence.Code("Services/ExplainsCheck.cs"),
            Layer: "The check names the doc and the paths that changed; whether the doc is still right is beyond it.",
            Reason: "The moment the code moves is the one moment somebody knows what the doc should now say.")
    };

    public static readonly Claim Format = new("A project's doc conventions, frontmatter and naming among them, are in its writing-docs reference.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 6,
            "Your project's doc conventions, frontmatter and naming among them, are in `" + Conventions + "`.")
    ])
    {
        Spec = ["K29", "K31"],
        Truth = new(Evidence.Run("`" + Conventions + "` exists here and under `Scaffold/`, the scaffold dydo ships; both open on Frontmatter and hold Naming Conventions."),
            Layer: "`dydo check` rejects a doc that breaks a convention and teaches none of them beforehand.",
            Reason: "A writer that learns the shape from the reference passes the check on its first run.")
    };

    public static readonly Claim Prove = new("A docs change is proved with dydo check, and dydo fix is run after a document is added, moved or renamed.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 7,
            "Prove a docs change with `dydo check` and the checks your brief names, and run `dydo fix` after you add, move or rename a document.")
    ])
    {
        Spec = ["K31", "D5"],
        Truth = new(Evidence.Command("dydo check"),
            Layer: "The CLI runs both on request and neither by itself after an edit.",
            Reason: "A moved doc leaves dead links behind in files its writer never opened.")
    };

    public static readonly Claim DrFields = new("A new decision record carries applies or new-ground, and one weighed line for each tension it resolved.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementDocs, 8,
            "A new decision record you write carries `applies:` (or `new-ground`) and one `weighed: <slug> over <slug>, because <reason>` for each tension it resolved.")
    ])
    {
        Spec = ["L1"],
        Truth = new(Evidence.Open("No check reads either field: `Services`, `Commands` and `Rules` hold no `weighed` or `new-ground`, and neither copy of `" + Conventions + "` names them. " +
                                  "Whether a decision record must carry them as the system runs is not known."),
            Layer: "`dydo check` validates a decision record's status and date, and nothing about how it ties to the law.",
            Reason: "A decision that names what it weighed can be found again when the same tension returns.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
