using System.Xml;
using System.Xml.Linq;

namespace XmlValidator;

public class XmlValidator
{
    public bool DetermineXml(string xml)
    {
        // early exit if early format is not satisfied
        if (string.IsNullOrWhiteSpace(xml) || !xml.StartsWith("<") || !xml.EndsWith(">"))
            return false;

        try
        {
            // leverage C# Linq XML document class
            var xDoc = XDocument.Parse(xml);
            return ValidateNodes(xDoc.Root);
        }
        catch (Exception)
        {
            // ignore exception, consider malformed xml
            return false;
        }
    }

    private bool ValidateNodes(XElement? node)
    {
        if (node == null || node.HasAttributes)
            return false;

        var isValid = true;
        foreach (var childNode in node.Nodes())
        {
            switch (childNode.NodeType)
            {
                case XmlNodeType.Element:
                    // loop through child nodes first, leverage recursive call
                    isValid &= ValidateNodes(childNode as XElement);
                    break;
                case XmlNodeType.Text:
                    // a text node means we've entered the value level of the element, skip validation
                    continue;
                default:
                    // we don't care about other forms of nodes for this exercise
                    return false;
            }
        }

        // convert the string once to a Span for performance gains
        // this might be too fancy, we can still revert to plain old string
        var nodeString = node.ToString(SaveOptions.DisableFormatting).AsSpan();
        if (nodeString.IsEmpty || nodeString.IsWhiteSpace())
            return false;

        // find the first end tag, grab the node name here
        var openingTag = nodeString[1..nodeString.IndexOf('>')];
        if (openingTag.IsEmpty || openingTag.IsWhiteSpace())
            return false;

        // manually construct the end tag and ensure it's the end of the string
        isValid &= nodeString.EndsWith(string.Concat("</", openingTag, ">"));
        return isValid;
    }
}
