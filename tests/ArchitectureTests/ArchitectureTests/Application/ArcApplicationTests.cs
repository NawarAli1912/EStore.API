using Application;
using MediatR;
using NetArchTest.Rules;
using System.Reflection;

namespace ArchitectureTests.Application;

public class ArcApplicationTests
{
    private static readonly Assembly ApplicationAssembly = typeof(DependencyInjection).Assembly;
    private const string InfrastructureNamespace = "Infrastructure";
    private const string PresentationNamespace = "Presentation";
    private const string DomainNamespace = "Domain";

    [Fact]
    public void Handlers_Should_BeSealed()
    {
        var results = Types.InAssembly(ApplicationAssembly)
           .That()
           .ImplementInterface(typeof(IRequestHandler<,>))
           .Should()
           .BeSealed()
           .GetResult();


        Assert.True(results.IsSuccessful);
    }

    [Fact]
    public void ApplicationProject_Should_NotHaveDepeendencyOnOtherProject()
    {
        var otherProjects = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace
        };

        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Handlers_Should_HaveDepenedcyOnDomainProject()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();


        Assert.True(result.IsSuccessful);
    }

}
