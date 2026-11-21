using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// The four truth questions as build checks (DYD-1295 317be5f7). On a truth-checked skill: each line is
/// read by an actor the skill belongs to; each claim it states carries its evidence, layer and reason;
/// every <c>dydo …</c> it names exists and stands in code; and a claim still open is withheld, alone on
/// its line. On every claim that carries a <see cref="Truth"/>, wherever its lines sit
/// (<see cref="Answers"/>): no field is empty, the reason is in words of its own, and an open claim
/// reaches no built file. <see cref="CheckPaths"/> runs against the repository root, where a code-path
/// evidence must exist.
/// </summary>
internal static partial class TruthCheck
{
    // Words that carry no reason beside the stop words: the ones a sentence leans on to point at its rule or its citation.
    private static readonly HashSet<string> Pointers = new(StringComparer.Ordinal)
    {
        "per", "see", "says", "said", "spec", "rule", "rules", "claim", "dr", "decision", "record", "section", "required", "requires"
    };

    [GeneratedRegex("[a-z0-9]+")]
    private static partial Regex Words();

    [GeneratedRegex("^(?:[a-z]{1,2})?[0-9]+$")]
    private static partial Regex CitationWord();

    [GeneratedRegex(@"^([0-9]{1,9})\. ")]
    private static partial Regex ListItem();

    [GeneratedRegex(@"^ {0,3}(?:`{3,}|~{3,})")]
    private static partial Regex Fence();

    /// <summary>The lines of <paramref name="skill"/> an open claim withholds from its files.</summary>
    public static HashSet<Placed> Check(SkillSpec skill, IReadOnlyList<Placed> lines, IReadOnlyDictionary<string, Claim> claims)
    {
        var withheld = new HashSet<Placed>();
        foreach (var placed in lines)
        {
            ReadBy(skill, placed);
            foreach (var id in placed.Claims.Where(id => claims[id].Truth is null))
                throw new InvalidDataException($"claim:{id} is stated by {skill.Name}, a truth-checked skill, and carries no Truth: its evidence, layer and reason");
            if (!placed.Claims.Any(id => claims[id].Truth!.Evidence.Kind == EvidenceKind.Open)) continue;
            if (placed.Claims.Count > 1)
                throw new InvalidDataException(
                    $"{skill.Name}: the line of claim:{placed.Owner} also states {string.Join(", ", placed.Claims.Skip(1).Select(id => "claim:" + id))}, " +
                    "and one of them is open; an open claim is stated alone, so its line can be withheld alone");
            withheld.Add(placed);
        }
        foreach (var placed in lines.Where(placed => !withheld.Contains(placed)))
            if (Miscommanded($"{skill.Name}: the line of claim:{placed.Owner}", placed.Line.Text) is { } problem)
                throw new InvalidDataException(problem);
        return withheld;
    }

    /// <summary>
    /// The first <c>dydo …</c> command <paramref name="text"/> names that the CLI does not have, or names outside
    /// code, as the failure of <paramref name="where"/>; or none. A claim line is read, then each whole built file.
    /// </summary>
    public static string? Miscommanded(string where, string text)
    {
        if (CommandNames.Missing(text).FirstOrDefault() is { } missing) return $"{where} names {missing}";
        return CommandNames.InProse(text).FirstOrDefault() is { } prose
            ? $"{where} names `{prose}` outside code; a command is written in code, where the build checks it against the CLI"
            : null;
    }

    /// <summary>
    /// Every claim that carries a <see cref="Truth"/> answers it, whichever skills state it and even when none does.
    /// An open claim reaches no built file: its line sits only in a <paramref name="truthChecked"/> skill, which
    /// withholds it, and it is no machine's move, whose arrow nothing could withhold.
    /// </summary>
    public static void Answers(ClaimIndex claims, IReadOnlySet<Enum> truthChecked)
    {
        foreach (var (id, claim) in claims.Claims)
        {
            if (claim.Truth is not { } truth) continue;
            Answered(id, claim, truth);
            if (truth.Evidence.Kind != EvidenceKind.Open) continue;
            if (claim.Transition is { } move)
                throw new InvalidDataException(
                    $"claim:{id} is open, and it moves {Names.Of(move.From)} to {Names.Of(move.To)}; a machine's arrow is built, so an open claim is no move");
            foreach (var (line, _) in claims.LinesOf(claim).Where(stated => !truthChecked.Contains(stated.Line.Skill)))
                throw new InvalidDataException(
                    $"claim:{id} is open, and its line in {Names.Kebab(line.Skill)} would state it; only a truth-checked skill withholds an open claim's line");
        }
    }

    /// <summary>
    /// Each code-path evidence a claim carries names, from <paramref name="root"/>, a file or a folder that is there:
    /// never an absolute path or a path through <c>..</c>. The builder asks no other tool, so whether git tracks the
    /// path is the suite's to check, over the committed tree.
    /// </summary>
    public static void CheckPaths(Model model, string root)
    {
        foreach (var (claim, id) in ModelIndex.ClaimIds(model.Kinds))
            if (claim.Truth is { Evidence: { Kind: EvidenceKind.Code, Text: var path } } && !Exists(root, path))
                throw new InvalidDataException($"claim:{id}: its code evidence, {path}, is no path in the repository");
    }

    /// <summary>
    /// A built file numbers each list 1, 2, 3 (DYD-1295 519126ab): the first item that repeats or skips a
    /// number, whether a steps slot or a verbatim line wrote it, or none. A fenced block is not read.
    /// </summary>
    public static string? Misnumbered(string path, string text)
    {
        var (previous, fenced, lines) = (0, false, text.Split('\n'));
        for (var index = 0; index < lines.Length; index++)
        {
            if (Fence().IsMatch(lines[index])) fenced = !fenced;
            if (fenced || lines[index].Length == 0 || lines[index][0] == ' ') continue;
            var item = ListItem().Match(lines[index]);
            var number = item.Success ? int.Parse(item.Groups[1].Value) : 0;
            if (item.Success && number != previous + 1)
                return $"{path}:{index + 1} numbers a list item {number} after {previous}; a list is numbered 1, 2, 3";
            previous = number;
        }
        return null;
    }

    // A path names a file or a folder from the root; an anchor after `#` or a line after `:` is not part of it.
    private static bool Exists(string root, string path)
    {
        var named = path.Split('#', ':')[0];
        if (Path.IsPathRooted(named) || named.Split('/', '\\').Contains("..")) return false;
        var file = Path.Combine(root, named);
        return File.Exists(file) || Directory.Exists(file);
    }

    private static void ReadBy(SkillSpec skill, Placed placed)
    {
        if (placed.Line.Reader is not { } reader)
            throw new InvalidDataException($"{skill.Name}: the line of claim:{placed.Owner} names no reader; a truth-checked line names the actor who acts on it");
        if (!skill.Readers.Contains(reader))
            throw new InvalidDataException(
                $"{skill.Name}: the line of claim:{placed.Owner} is read by {Names.Of(reader)}, and this skill belongs to " +
                $"{string.Join(", ", skill.Readers.Select(Names.Of))}; a line belongs in the skill of the reader who acts on it");
    }

    /// <summary>
    /// No field is empty, a command evidence names a real command, and the reason is its own: of its content
    /// words, the ones that are no English stop word (<see cref="StopWords"/>), pointer or citation, at least
    /// half are in neither the rule nor its spec ids (contract A3d). That is a floor, and padding with
    /// content words still clears it. Whether those words say what the claim prevents or enables is a
    /// reviewer's to judge, claim by claim, from the review list the why docs are.
    /// </summary>
    private static void Answered(string id, Claim claim, Truth truth)
    {
        foreach (var (name, text) in new[] { ("evidence", truth.Evidence.Text), ("layer", truth.Layer), ("reason", truth.Reason) })
            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidDataException($"claim:{id} has an empty {name}");
        if (truth.Evidence.Kind == EvidenceKind.Command && CommandNames.MissingIn(truth.Evidence.Text).FirstOrDefault() is { } missing)
            throw new InvalidDataException($"claim:{id}: its command evidence names {missing}");
        var (own, words) = OwnWords(truth.Reason, claim);
        if (own == 0 || own * 2 < words)
            throw new InvalidDataException(
                $"claim:{id}: its reason restates its rule or its spec citation, with {own} of {words} words its own; at least half must be, " +
                "and a reason says what the claim prevents or enables");
    }

    // How many of the reason's words carry meaning, and how many of those its rule and its citations do not already hold.
    // Its lines are not held against it: a line says what to do, and its reason may well use the same words to say why.
    private static (int Own, int Words) OwnWords(string reason, Claim claim)
    {
        var said = WordsOf(string.Join(' ', [claim.Rule, .. claim.Spec]));
        var words = WordsOf(reason).Where(word => !StopWords.English.Contains(word) && !Pointers.Contains(word) && !CitationWord().IsMatch(word)).ToArray();
        return (words.Count(word => !said.Contains(word)), words.Length);
    }

    private static HashSet<string> WordsOf(string text) =>
        Words().Matches(text.ToLowerInvariant()).Select(word => word.Value).ToHashSet(StringComparer.Ordinal);
}
