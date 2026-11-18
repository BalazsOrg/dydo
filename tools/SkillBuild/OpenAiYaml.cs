namespace DynaDocs.SkillBuild;

/// <summary>
/// The <c>interface</c> and <c>dependencies</c> blocks of a skill's <c>agents/openai.yaml</c> (K26, K27),
/// each the block's body as YAML; the builder indents it under its key.
/// </summary>
public sealed record OpenAiYaml(string? Interface = null, string? Dependencies = null);
