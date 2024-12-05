using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Domain.Entities;
using VendingMachine.Services;

namespace UnitTestingVendingMachine
{
    [TestFixture]
    public class CoffeeServiceTest
    {
        private CoffeeService _coffeeService;

        [SetUp]
        public void SetUp()
        {
            _coffeeService = new CoffeeService();
        }

        [Test]
        public void GetAvailableCoffees_ShouldReturnAllCoffees()
        {
            // Act
            var coffees = _coffeeService.GetAvailableCoffees();

            // Assert
            Assert.AreEqual(4, coffees.Count, "Debe retornar 4 tipos de cafés.");
            CollectionAssert.AllItemsAreUnique(coffees.Select(c => c.Type), "Cada tipo de café debe ser único.");
        }

        [Test]
        public void UpdateStock_ShouldDecreaseStockCorrectly()
        {
            // Arrange
            var purchase = new Dictionary<string, int> { { "Americano", 2 }, { "Latte", 3 } };

            // Act
            _coffeeService.UpdateStock(purchase);
            var coffees = _coffeeService.GetAvailableCoffees();

            // Assert
            Assert.AreEqual(8, coffees.First(c => c.Type == "Americano").Stock, "El stock de Americano debe ser 8.");
            Assert.AreEqual(7, coffees.First(c => c.Type == "Latte").Stock, "El stock de Latte debe ser 7.");
        }

        [Test]
        public void UpdateStock_ShouldNotUpdateStock_WhenQuantityExceedsStock()
        {
            // Arrange
            var purchase = new Dictionary<string, int> { { "Capuccino", 10 } };

            // Act
            _coffeeService.UpdateStock(purchase);
            var coffees = _coffeeService.GetAvailableCoffees();

            // Assert
            Assert.AreEqual(8, coffees.First(c => c.Type == "Capuccino").Stock, "El stock de Capuccino no debe cambiar si la cantidad supera el stock.");
        }

        [Test]
        public void CalculateTotalCost_ShouldReturnCorrectTotal()
        {
            // Arrange
            var purchase = new Dictionary<string, int> { { "Americano", 1 }, { "Mocaccino", 2 } };

            // Act
            var totalCost = _coffeeService.CalculateTotalCost(purchase);

            // Assert
            Assert.AreEqual(3950, totalCost, "El costo total debe ser 3950.");
        }

        [Test]
        public void CalculateTotalCost_ShouldThrowException_ForInvalidCoffeeType()
        {
            // Arrange
            var purchase = new Dictionary<string, int> { { "Espresso", 1 } };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _coffeeService.CalculateTotalCost(purchase));
            Assert.AreEqual("Coffee type 'Espresso' not found.", ex.Message, "Debe lanzar una excepción para un tipo de café inválido.");
        }

        [Test]
        public void UpdateStock_ShouldHandleEmptyPurchase()
        {
            // Arrange
            var emptyPurchase = new Dictionary<string, int>();

            // Act
            _coffeeService.UpdateStock(emptyPurchase);
            var coffees = _coffeeService.GetAvailableCoffees();

            // Assert
            CollectionAssert.AllItemsAreNotNull(coffees.Select(c => c.Stock), "El stock no debe cambiar si la compra está vacía.");
        }
    }
}
