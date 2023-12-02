-- Create root category
INSERT INTO Category.Categories (Id, Name, ParentCategoryId)
VALUES
    (NEWID(), 'Electronics', NULL);

-- Create subcategories for Electronics
INSERT INTO Category.Categories (Id, Name, ParentCategoryId)
VALUES
    (NEWID(), 'Mobile Devices', (SELECT Id FROM Category.Categories WHERE Name = 'Electronics')),
    (NEWID(), 'Computers', (SELECT Id FROM Category.Categories WHERE Name = 'Electronics')),
    (NEWID(), 'Audio and Headphones', (SELECT Id FROM Category.Categories WHERE Name = 'Electronics'));

-- Create subcategories for Mobile Devices
INSERT INTO Category.Categories (Id, Name, ParentCategoryId)
VALUES
    (NEWID(), 'Smartphones', (SELECT Id FROM Category.Categories WHERE Name = 'Mobile Devices')),
    (NEWID(), 'Tablets', (SELECT Id FROM Category.Categories WHERE Name = 'Mobile Devices')),
    (NEWID(), 'Wearables', (SELECT Id FROM Category.Categories WHERE Name = 'Mobile Devices'));

-- Create subcategories for Computers
INSERT INTO Category.Categories (Id, Name, ParentCategoryId)
VALUES
    (NEWID(), 'Laptops', (SELECT Id FROM Category.Categories WHERE Name = 'Computers')),
    (NEWID(), 'Desktops', (SELECT Id FROM Category.Categories WHERE Name = 'Computers')),
    (NEWID(), 'Accessories', (SELECT Id FROM Category.Categories WHERE Name = 'Computers'));

-- Create subcategories for Audio and Headphones
INSERT INTO Category.Categories (Id, Name, ParentCategoryId)
VALUES
    (NEWID(), 'Headphones', (SELECT Id FROM Category.Categories WHERE Name = 'Audio and Headphones')),
    (NEWID(), 'Speakers', (SELECT Id FROM Category.Categories WHERE Name = 'Audio and Headphones')),
    (NEWID(), 'Sound Systems', (SELECT Id FROM Category.Categories WHERE Name = 'Audio and Headphones'));

