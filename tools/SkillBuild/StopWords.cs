namespace DynaDocs.SkillBuild;

/// <summary>
/// English function words, which carry no content of their own. <see cref="TruthCheck"/> leaves them out
/// when it counts how much of a reason is the reason's own. The list is NLTK's English stop words, each
/// split at its apostrophe as the check splits a sentence, then the modals and connectives that list lacks.
/// </summary>
internal static class StopWords
{
    public static readonly HashSet<string> English = new(StringComparer.Ordinal)
    {
        "i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours", "yourself", "yourselves",
        "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "they", "them", "their", "theirs", "themselves",
        "what", "which", "who", "whom", "this", "that", "these", "those", "am", "is", "are", "was", "were", "be", "been", "being",
        "have", "has", "had", "having", "do", "does", "did", "doing", "a", "an", "the", "and", "but", "if", "or", "because", "as", "until", "while",
        "of", "at", "by", "for", "with", "about", "against", "between", "into", "through", "during", "before", "after", "above", "below",
        "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again", "further", "then", "once",
        "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such",
        "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don", "should", "now",
        "d", "ll", "m", "o", "re", "ve", "y", "ain", "aren", "couldn", "didn", "doesn", "hadn", "hasn", "haven", "isn", "ma",
        "mightn", "mustn", "needn", "shan", "shouldn", "wasn", "weren", "won", "wouldn",
        // Not in NLTK's list.
        "every", "also", "would", "could", "may", "might", "must", "shall", "cannot", "never", "always", "ever", "yet", "still", "even",
        "either", "neither", "whether", "without", "within", "upon", "since", "thus", "hence", "else"
    };
}
