using VendingMachine.Services;
using VendingMachine.Models;
using System.Collections.Generic;

namespace VendingMachine.Handlers
{
    public class PurchaseCoffeeHandler
    {
        private readonly ICoffeeService _coffeeService;
        private readonly IChangeCalculator _changeCalculator;

        public PurchaseCoffeeHandler(ICoffeeService coffeeService, IChangeCalculator changeCalculator)
        {
            _coffeeService = coffeeService;
            _changeCalculator = changeCalculator;
        }

        public PurchaseResponse Purchase(Dictionary<string, int> purchaseQuantities, decimal paidAmount, Dictionary<int, int> manualCoinsInput)
        {
            // Suma el ingreso manual al stock de monedas
            _changeCalculator.AddManualCoinsToStock(manualCoinsInput);

            var totalCost = _coffeeService.CalculateTotalCost(purchaseQuantities);

            if (paidAmount < totalCost)
            {
                return new PurchaseResponse
                {
                    Success = false,
                    Message = "Fondos insuficientes",
                    RemainingCoffees = _coffeeService.GetAvailableCoffees()
                };
            }

            var changeResult = _changeCalculator.CalculateChange(paidAmount, totalCost);

            if (!changeResult.Success)
            {
                return new PurchaseResponse
                {
                    Success = false,
                    Message = changeResult.ErrorMessage,
                    RemainingCoffees = _coffeeService.GetAvailableCoffees()
                };
            }

            _coffeeService.UpdateStock(purchaseQuantities);

            return new PurchaseResponse
            {
                Success = true,
                Message = "Compra exitosa",
                Change = new ChangeResponse
                {
                    Total = changeResult.Total,
                    Breakdown = changeResult.Breakdown
                },
                RemainingCoffees = _coffeeService.GetAvailableCoffees()
            };
        }
    }
}