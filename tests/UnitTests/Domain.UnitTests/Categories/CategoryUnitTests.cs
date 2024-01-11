using Domain.Categories;
using Domain.Products;

namespace Domain.UnitTests.Categories;
public sealed class CategoryUnitTests
{
    [Fact]
    public void CreateCategory_WithValidData_CreatesCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        string name = "Electronics";
        var parentId = Guid.NewGuid();

        // Act
        var category = Category.Create(id, name, parentId);

        // Assert
        Assert.Equal(id, category.Id);
        Assert.Equal(name, category.Name);
        Assert.Equal(parentId, category.ParentCategoryId);
    }

    [Fact]
    public void AssignProducts_ToCategory_AssignsCorrectly()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics");
        var products = new List<Product>
        {
        TestDataFactory.CreateProduct("P1"),
        TestDataFactory.CreateProduct("P2"),
        };

        // Act
        category.AssignProducts(products);

        // Assert
        Assert.Equal(products.Count, category.Products.Count);
        foreach (var product in products)
        {
            Assert.Contains(product, category.Products);
        }
    }

    [Fact]
    public void UpdateCategory_WithNewData_UpdatesCorrectly()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics");
        string newName = "Home Appliances";
        var newParentId = Guid.NewGuid();

        // Act
        category.Update(newName, newParentId, false);

        // Assert
        Assert.Equal(newName, category.Name);
        Assert.Equal(newParentId, category.ParentCategoryId);
    }

    [Fact]
    public void BuildCategoryTree_SingleLevel_BuildsCorrectly()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parentCategory = Category.Create(parentId, "ParentCategory");
        var childCategories = new List<Category>
    {
        Category.Create(Guid.NewGuid(), "Child1", parentId),
        Category.Create(Guid.NewGuid(), "Child2", parentId)
    };
        var allCategories = new List<Category> { parentCategory }.Concat(childCategories).ToList();

        // Act
        var tree = Category.BuildCategoryTree(allCategories, parentId);

        // Assert
        Assert.Equal(childCategories.Count, tree.Count);
        foreach (var child in childCategories)
        {
            Assert.Contains(tree, c => c.Id == child.Id);
        }
    }

    [Fact]
    public void BuildCategoryTree_MultiLevel_BuildsCorrectly()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parentCategory = Category.Create(parentId, "ParentCategory");

        var childCategory1 = Category.Create(Guid.NewGuid(), "Child1", parentId);
        var childCategory2 = Category.Create(Guid.NewGuid(), "Child2", parentId);

        var grandChildCategory1 = Category.Create(Guid.NewGuid(), "GrandChild1", childCategory1.Id);
        var grandChildCategory2 = Category.Create(Guid.NewGuid(), "GrandChild2", childCategory1.Id);

        var allCategories = new List<Category> { parentCategory, childCategory1, childCategory2, grandChildCategory1, grandChildCategory2 };

        // Act
        var tree = Category.BuildCategoryTree(allCategories, parentId);

        // Assert
        Assert.Equal(2, tree.Count); // Two children of the parent
        var child1Tree = tree.FirstOrDefault(c => c.Id == childCategory1.Id);
        Assert.NotNull(child1Tree);
        Assert.Equal(2, child1Tree.SubCategories.Count); // Two grandchildren of childCategory1
    }

    [Fact]
    public void BuildCategoryTree_NoChildren_ReturnsEmptySubCategories()
    {
        // Arrange
        var parentCategory = Category.Create(Guid.NewGuid(), "ParentCategory");
        var allCategories = new List<Category> { parentCategory };

        // Act
        var tree = Category.BuildCategoryTree(allCategories, parentCategory.Id);

        // Assert
        Assert.Empty(tree);
    }

    [Fact]
    public void BuildCategoryTree_WithUnrelatedCategories_ExcludesThem()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var unrelatedCategoryId = Guid.NewGuid();
        var parentCategory = Category.Create(parentId, "ParentCategory");
        var childCategory = Category.Create(Guid.NewGuid(), "Child", parentId);
        var unrelatedCategory = Category.Create(unrelatedCategoryId, "Unrelated");

        var allCategories = new List<Category> { parentCategory, childCategory, unrelatedCategory };

        // Act
        var tree = Category.BuildCategoryTree(allCategories, parentId);

        // Assert
        Assert.Single(tree);
        Assert.DoesNotContain(tree, c => c.Id == unrelatedCategoryId);
    }

}
