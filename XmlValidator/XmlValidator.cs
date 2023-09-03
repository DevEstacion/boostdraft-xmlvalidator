using BenchmarkDotNet.Attributes;

namespace XmlValidator;

public static class XmlValidator
{
    private static readonly char[] _invalidCharacters;

    static XmlValidator()
    {
        // define a bigger set of invalid characters once available
        _invalidCharacters = new[] {' ', '=', '\\', '/', '"', '\''};
    }

    [Benchmark]
    public static bool DetermineXml(string xml)
    {
        // early exit if early format is not satisfied
        if (string.IsNullOrWhiteSpace(xml) || !xml.StartsWith("<") || !xml.EndsWith(">"))
            return false;

        try
        {
            // use span for performance gains and prevent recreation of
            // string instances and memory usages. useful for big xml's
            var charSpans = xml.AsSpan();
            return PerformCharacterLoop(charSpans);
        }
        catch (Exception)
        {
            // ignore exception, consider malformed xml
            return false;
        }
    }

    /// <summary>
    ///     The main logic is to always find the first '</' and we should be able to
    /// trace back the opening tag by finding the last inserted '<' symbol on the
    /// lessThanCharElementStarts Stack.
    /// </summary>
    private static bool PerformCharacterLoop(ReadOnlySpan<char> charSpans)
    {
        var isValid = true;
        var lessThanCharElementStarts = new Stack<int>();
        var lastLessThanCharIndex = 0;
        var lastCharArrayIndex = charSpans.Length - 1;
        for (var index = 0; index < charSpans.Length; index++)
        {
            var currentChar = charSpans[index];
            // preserve the locations of the found opening tags in a stack
            // ensure first in first out to ensure match with the found closing tag
            if (currentChar == '<' && index != lastCharArrayIndex && charSpans[index + 1] != '/')
            {
                lessThanCharElementStarts.Push(index);
            }

            // when we find the closing tag, we want to ensure that there's still items
            // in the stack, an empty stack means an unmatched closing tag.
            if (currentChar == '<' && index != lastCharArrayIndex && charSpans[index + 1] == '/')
            {
                if (lessThanCharElementStarts.Count == 0)
                    return false;

                lastLessThanCharIndex = index;
            }

            // this means we have found the '>' of the closing tag and we can now perform
            // the validation of the opening and closing tags
            if (currentChar == '>' && lastLessThanCharIndex > 0)
            {
                isValid &= PerformValidation(charSpans, lessThanCharElementStarts.Pop(), lastLessThanCharIndex, index);
                // reset the last closing tag to zero to continue the find for the next one
                if (isValid)
                    lastLessThanCharIndex = 0;
                else
                    return false;
            }

            // if this case is satisfied, it means we only found a closing tag
            if (lessThanCharElementStarts.Count > 0 && index == lastCharArrayIndex)
                return false;
        }

        return isValid;
    }

    private static bool PerformValidation(ReadOnlySpan<char> charSpans, int startIndex, int endIndexLeft,
        int endIndexRight)
    {
        // gets the tag name without the greater than and less than characters
        var closingTagName = charSpans[(endIndexLeft + 2)..endIndexRight];
        // check for common cases as well as invalid characters
        if (closingTagName.IsEmpty
            || closingTagName.IsWhiteSpace()
            || closingTagName.Contains(_invalidCharacters, StringComparison.InvariantCulture))
        {
            return false;
        }

        // check if the closing tag doesn't have any attributes and also matches the found closing tag name
        if (!charSpans.Slice(endIndexLeft, closingTagName.Length + 3)
                .SequenceEqual(string.Concat("</", closingTagName, ">")))
        {
            return false;
        }

        // check if the opening tag matches the closing tag name
        if (!charSpans.Slice(startIndex, closingTagName.Length + 2)
                .SequenceEqual(string.Concat("<", closingTagName, ">")))
        {
            return false;
        }

        return true;
    }
}
