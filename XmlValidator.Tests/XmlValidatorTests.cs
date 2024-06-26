namespace XmlValidator.Tests;

public class XmlValidatorTests
{
    [Theory]
    [InlineData("<Design><Code></Code></Design>", true)]
    [InlineData("<Design><Code>hello world</Code></Design>", true)]
    [InlineData("<Design><Code>hello world</Code></Design><People>", false)]
    [InlineData("<People><Design><Code>hello world</People></Code></Design>", false)]
    [InlineData("<People age=\"1\">hello world</People>", false)]
    [InlineData("<tutorial date=\"01/01/2000\">xml</tutorial>", false)]
    public void DetermineXml_Tests(string xml, bool isValid)
    {
        // act
        var result = XmlValidator.DetermineXml(xml);

        // assert
        Assert.Equal(result, isValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("<Design><>hello world</></Design>")]
    [InlineData("<Design><  >hello world</></Design>")]
    [InlineData("<Design><  >hello world</  ></Design>")]
    [InlineData("<Design/Design>")]
    [InlineData("<Design></    Design>")]
    [InlineData("<Design></Design    >")]
    [InlineData("<Design    ></Design>")]
    [InlineData("<    Design></Design>")]
    [InlineData("<Design>/Design>")]
    [InlineData("<Design</Design>")]
    [InlineData("</Design>")]
    [InlineData("<Design>")]
    [InlineData("< >")]
    [InlineData("<Test attrib=false>")]
    [InlineData("<Test attrib='false'>")]
    [InlineData("<Test attrib=\"false\">")]
    [InlineData("<Test     ")]
    [InlineData("<     Test")]
    [InlineData("</ >")]
    public void DetermineXml_InvalidInputStressTest(string xml)
    {
        // act
        var result = XmlValidator.DetermineXml(xml);

        // assert
        Assert.False(result);
    }
}
