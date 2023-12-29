using Application;
using MediatR;
using NetArchTest.Rules;
using System.Reflection;

namespace ArchitectureTests.Application;

public class ArcApplicationTests
{
    private static readonly Assembly ApplicationAssembly = typeof(DependencyInjection).Assembly;

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
}
