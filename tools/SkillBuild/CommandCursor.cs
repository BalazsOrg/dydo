using System.CommandLine;

namespace DynaDocs.SkillBuild;

/// <summary>
/// Where <see cref="CommandNames"/> stands in the command tree while it reads a command's words: the
/// command reached, as typed so far, the recursive options its ancestors pass down, and whether the
/// next word is the value of the option just named.
/// </summary>
internal sealed record CommandCursor(Command Node, string Path, IReadOnlyList<Option> Inherited, bool OptionValue);
