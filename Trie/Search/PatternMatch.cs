﻿using System.Diagnostics;

namespace BlueHeron.Collections.Trie.Search;

/// <summary>
/// An extended <see cref="List{CharMatch}"/> that represents a pattern of characters, including wildcards.
/// </summary>
[DebuggerStepThrough()]
public class PatternMatch : List<CharMatch>
{
    #region Objects and variables

    private const string errNoFirstOrLastWildCard = "First and last character should not be null when Type is PatternMatchType.IsFragment.";
    internal const string _DOTSTAR = ".*";

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="PatternMatchType"/> to use. Default: <see cref="PatternMatchType.IsPrefix"/>.
    /// </summary>
    public PatternMatchType Type { get; set; }

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new, empty <see cref="PatternMatch"/>.
    /// </summary>
    public PatternMatch(): base(){}

    /// <summary>
    /// Creates a new <see cref="PatternMatch"/> from the given <see cref="IEnumerable{char?}"/>.
    /// A null char in the collection will yield a wildcard.
    /// </summary>
    [DebuggerStepThrough()]
    public PatternMatch(IEnumerable<char?> pattern, PatternMatchType type) : base(pattern.ToCharMatchArray())
    {
        if (type == PatternMatchType.IsFragment && (pattern.First() == null || pattern.Last() == null))
        {
            throw new ArgumentException(errNoFirstOrLastWildCard);
        }
        Type = type;
    }

    /// <summary>
    /// Creates a new <see cref="PatternMatch"/> from the given <see cref="IEnumerable{CharMatch}"/>.
    /// </summary>
    public PatternMatch(IEnumerable<CharMatch> collection, PatternMatchType type) : base(collection) {
        if (type == PatternMatchType.IsFragment && (collection.First().Primary == null || collection.Last().Primary == null))
        {
            throw new ArgumentException(errNoFirstOrLastWildCard);
        }
        Type = type;
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Adds a <see cref="CharMatch"/> to the collection.
    /// </summary>
    /// <param name="character">The character to match</param>
    public void Add(char character)
    {
        Add(new CharMatch(character));
    }

    /// <summary>
    /// Adds a <see cref="CharMatch"/> to the collection.
    /// </summary>
    /// <param name="character">The character to match</param>
    /// <param name="alternatives">Option array of alternative characters to match</param>
    public void Add(char character, char[]? alternatives)
    {
        Add(new CharMatch(character, alternatives));
    }

    /// <summary>
    /// Creates a <see cref="PatternMatch"/> of type <see cref="PatternMatchType.IsFragment"/> representing the given fragment.
    /// </summary>
    /// <param name="fragment">The fragment</param>
    /// <returns>A <see cref="PatternMatch"/></returns>
    public static PatternMatch FromFragment(string fragment)
    {
        return new PatternMatch(fragment.ToCharMatchArray(), PatternMatchType.IsFragment);
    }

    /// <summary>
    /// Creates a <see cref="PatternMatch"/> of type <see cref="PatternMatchType.IsPrefix"/> representing the given prefix.
    /// </summary>
    /// <param name="prefix">The prefix</param>
    /// <returns>A <see cref="PatternMatch"/></returns>
    public static PatternMatch FromPrefix(string? prefix)
    {
        return new PatternMatch(prefix.ToCharMatchArray(), PatternMatchType.IsPrefix);
    }

    /// <summary>
    /// Creates a <see cref="PatternMatch"/> of type <see cref="PatternMatchType.IsWord"/> representing the given word.
    /// </summary>
    /// <param name="word">The word</param>
    /// <returns>A <see cref="PatternMatch"/></returns>
    public static PatternMatch FromWord(string word)
    {
        return new PatternMatch(word.ToCharMatchArray(), PatternMatchType.IsWord);
    }

    /// <summary>
    /// Overridden to return this <see cref="PatternMatch"/> as a regex expression.
    /// </summary>
    /// <returns>A regex expression</returns>
    public override string ToString()
    {
        if (Count == 0)
        {
            return string.Empty;
        }
        var strRegex = string.Join(string.Empty, this.Select(c => c.ToString()));

        return Type switch
        {
            PatternMatchType.IsWord => strRegex,
            PatternMatchType.IsFragment => _DOTSTAR + strRegex + _DOTSTAR,
            _ => strRegex + _DOTSTAR // prefix
        };
    }

    #endregion
}