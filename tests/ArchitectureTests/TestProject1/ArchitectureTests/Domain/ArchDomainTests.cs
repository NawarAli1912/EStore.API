using Domain.Products;
using NetArchTest.Rules;
using SharedKernel.Primitives;
using System.Reflection;

namespace ArchitectureTests.Domain;
public class ArchDomainTests
{
    private static readonly Assembly DomainAssembly = typeof(Product).Assembly;
    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        var results = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(results.IsSuccessful);
    }

    [Fact]
    public void DomainEvents_Should_HaveDomainEventPostfix()
    {
        var results = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        Assert.True(results.IsSuccessful);
    }

    [Fact]
    public void Entities_Should_HavePrivateParamterlessConstructor()
    {
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var fallingTypes = new List<Type>();

        foreach (var item in entityTypes)
        {
            var constructors = item.GetConstructors(BindingFlags.Instance |
                                                    BindingFlags.NonPublic);

            if (!constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                fallingTypes.Add(item);
            }
        }

        Assert.Empty(fallingTypes);
    }
}
