namespace VendingMachine.Services
{
    using VendingMachine.Domain.Entities;
    using System.Collections.Generic;

    public interface ICoffeeService
    {
        List<Coffee> GetAvailableCoffees();
        void UpdateStock(Dictionary<string, int> purchaseQuantities);
        decimal CalculateTotalCost(Dictionary<string, int> purchaseQuantities);
    }
}