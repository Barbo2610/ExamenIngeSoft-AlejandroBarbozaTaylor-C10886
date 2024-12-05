using System.Collections.Generic;
using System.Linq;
using VendingMachine.Domain.Entities;

namespace VendingMachine.Services
{
    public class ChangeCalculatorService : IChangeCalculator
    {
        private readonly Dictionary<int, int> _availableCoins;

        public ChangeCalculatorService()
        {
            _availableCoins = new Dictionary<int, int>
            {
                { 500, 20 },
                { 100, 30 },
                { 50, 50 },
                { 25, 25 }
            };
        }

        public Change CalculateChange(decimal paidAmount, decimal totalCost)
        {
            var change = new Change();
            var changeAmount = paidAmount - totalCost;

            if (changeAmount < 0)
            {
                change.Success = false;
                change.ErrorMessage = "El monto ingresado es insuficiente.";
                return change;
            }

            var remainingChange = changeAmount;
            var breakdown = new Dictionary<int, int>();
            var tempCoins = new Dictionary<int, int>(_availableCoins);

            foreach (var denomination in tempCoins.Keys.OrderByDescending(k => k))
            {
                if (remainingChange == 0) break;

                var count = (int)(remainingChange / denomination);
                count = Math.Min(count, tempCoins[denomination]);

                if (count > 0)
                {
                    breakdown[denomination] = count;
                    remainingChange -= count * denomination;
                    tempCoins[denomination] -= count;
                }
            }

            if (remainingChange == 0)
            {
                foreach (var kvp in breakdown)
                {
                    _availableCoins[kvp.Key] -= kvp.Value;
                }

                change.Total = changeAmount;
                change.Breakdown = breakdown;
                change.Success = true;
            }
            else
            {
                change.Success = false;
                change.ErrorMessage = "No hay suficientes monedas para el vuelto.";
            }

            return change;
        }

        public void AddManualCoinsToStock(Dictionary<int, int> manualCoinsInput)
        {
            foreach (var coin in manualCoinsInput)
            {
                if (_availableCoins.ContainsKey(coin.Key))
                {
                    _availableCoins[coin.Key] = Math.Max(0, _availableCoins[coin.Key] + coin.Value); // Evitar valores negativos
                }
                else if (coin.Value > 0)
                {
                    _availableCoins[coin.Key] = coin.Value;
                }
            }
        }
    }
}