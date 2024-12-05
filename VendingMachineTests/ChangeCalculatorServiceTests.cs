using System.Collections.Generic;
using NUnit.Framework;
using VendingMachine.Services;
using VendingMachine.Domain.Entities;

namespace UnitTestingVendingMachine
{
    [TestFixture]
    public class ChangeCalculatorServiceTests
    {
        private ChangeCalculatorService _changeCalculator;

        [SetUp]
        public void SetUp()
        {
            // Inicializamos el servicio antes de cada prueba
            _changeCalculator = new ChangeCalculatorService();
        }

        [Test]
        public void CalculateChange_ShouldReturnCorrectChange_WhenPaidAmountIsSufficient()
        {
            // Arrange
            decimal paidAmount = 1000m;
            decimal totalCost = 750m;

            // Act
            Change result = _changeCalculator.CalculateChange(paidAmount, totalCost);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(250m, result.Total);
            Assert.AreEqual(2, result.Breakdown[100]);
            Assert.AreEqual(1, result.Breakdown[50]);
            Assert.IsFalse(result.Breakdown.ContainsKey(500));
            Assert.IsFalse(result.Breakdown.ContainsKey(25));
        }

        [Test]
        public void CalculateChange_ShouldReturnError_WhenPaidAmountIsInsufficient()
        {
            // Arrange
            decimal paidAmount = 700m;
            decimal totalCost = 750m;

            // Act
            Change result = _changeCalculator.CalculateChange(paidAmount, totalCost);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("El monto ingresado es insuficiente.", result.ErrorMessage);
        }

        [Test]
        public void AddManualCoinsToStock_ShouldIncreaseCoinStockCorrectly()
        {
            // Arrange
            var coinsToAdd = new Dictionary<int, int>
            {
                { 500, 10 },
                { 100, 5 },
                { 50, 3 },
                { 25, 2 }
            };

            // Act
            _changeCalculator.AddManualCoinsToStock(coinsToAdd);

            // Assert
            var result = _changeCalculator.CalculateChange(1000m, 750m);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(250m, result.Total);
            Assert.AreEqual(2, result.Breakdown[100]);
            Assert.AreEqual(1, result.Breakdown[50]);
        }

        [Test]
        public void CalculateChange_ShouldHandleExactPaymentCorrectly()
        {
            // Arrange
            decimal paidAmount = 750m;
            decimal totalCost = 750m;

            // Act
            Change result = _changeCalculator.CalculateChange(paidAmount, totalCost);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Total);
            Assert.IsEmpty(result.Breakdown);
        }

        [Test]
        public void CalculateChange_ShouldHandleEdgeCase_WithExactStock()
        {
            // Arrange
            decimal paidAmount = 1000m;
            decimal totalCost = 800m;

            _changeCalculator.AddManualCoinsToStock(new Dictionary<int, int>
            {
                { 100, -28 },
                { 50, -48 } // Ajustar stock a lo justo
            });

            // Act
            Change result = _changeCalculator.CalculateChange(paidAmount, totalCost);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Total);
            Assert.AreEqual(2, result.Breakdown[100]);
        }

        [Test]
        public void AddManualCoinsToStock_ShouldAddNewDenominations()
        {
            // Arrange
            var coinsToAdd = new Dictionary<int, int>
            {
                { 200, 5 } // Nueva denominación no existente previamente
            };

            // Act
            _changeCalculator.AddManualCoinsToStock(coinsToAdd);

            // Assert
            // Intentar un cambio con la nueva denominación
            var result = _changeCalculator.CalculateChange(1200m, 1000m);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200m, result.Total);
            Assert.AreEqual(1, result.Breakdown[200]);
        }
    }
}