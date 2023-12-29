using Infrastructure;
using NetArchTest.Rules;
using System.Reflection;

namespace ArchitectureTests.Infrastructure;
public class ArcInfrastructureTests
{
    private const string PresentationNamespace = "Presentation";
    private static readonly Assembly InfrastructureAssembly = typeof(DependencyInjection).Assembly;

    [Fact]
    public void InfrastructureProject_Should_NotHaveDepeendencyOnOtherProject()
    {
        var otherProjects = new[]
        {
            PresentationNamespace
        };

        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
