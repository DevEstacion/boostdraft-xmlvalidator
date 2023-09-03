using BenchmarkDotNet.Attributes;

namespace Benchmarker;

[MemoryDiagnoser(false)]
public class Benchmark
{
    private const string _xml = "<Design><Code></Code></Design>";

    [Benchmark]
    public bool Run()
    {
        return XmlValidator.XmlValidator.DetermineXml(_xml);
    }
}
