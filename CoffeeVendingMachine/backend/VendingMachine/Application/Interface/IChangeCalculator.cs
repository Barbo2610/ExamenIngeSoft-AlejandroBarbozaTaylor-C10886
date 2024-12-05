namespace VendingMachine.Services
{
    using VendingMachine.Domain.Entities;
    using System.Collections.Generic;

    public interface IChangeCalculator
    {
        Change CalculateChange(decimal paidAmount, decimal totalCost);
        void AddManualCoinsToStock(Dictionary<int, int> manualCoinsInput);
    }
}