using BenchmarkDotNet.Attributes;

namespace Benchmarker;

[MemoryDiagnoser(false)]
public class Benchmark
{
    private const string _xml = "<Design><Code></Code></Design>";
    private XmlValidator.XmlValidator _validator;

    [GlobalSetup]
    public void Setup()
    {
        this._validator = new XmlValidator.XmlValidator();
    }

    [Benchmark]
    public bool Run()
    {
        return this._validator.DetermineXml(_xml);
    }
}
