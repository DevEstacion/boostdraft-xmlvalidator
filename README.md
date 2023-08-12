# XmlValidator

## Summary
This XmlValidator class leverages C# [XDocument](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument?view=net-7.0) and its internal parsing mechanism to do preliminary validations of the XML string. It will then walk the tree of the XML string to find additional invalid and malformed XML styles as per the requirements of this exercise.

At its core, it will do a recursive call for every child until it finds the innermost child and walk back the tree.

## Unit Tests
The project has a XmlValidator.Tests project where valid and invalid tests are written to further verify the integrity of the code. New test scenarios and cases can be added to these tests for additional coverage.

The test ensures that the current functionality remains consistent and future implementations doesn't break anything.

## Technology Used

* Rider
* .netcore 7.0
* C# 11

## Author
Ronald Estacion \
[devestacion@skiff.com](mailto:devestacion@skiff.com) \
Github: https://github.com/DevEstacion \
LinkedIn: https://www.linkedin.com/in/ronaldestacion/