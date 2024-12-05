using VendingMachine.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace VendingMachine.Services
{
    public class CoffeeService : ICoffeeService
    {
        private readonly List<Coffee> _coffees;

        public CoffeeService()
        {
            // Simulación de base de datos en memoria.
            _coffees = new List<Coffee>
            {
                new Coffee { Type = "Americano", Stock = 10, Price = 950 },
                new Coffee { Type = "Capuccino", Stock = 8, Price = 1200 },
                new Coffee { Type = "Latte", Stock = 10, Price = 1350 },
                new Coffee { Type = "Mocaccino", Stock = 15, Price = 1500 }
            };
        }

        public List<Coffee> GetAvailableCoffees()
        {
            return _coffees;
        }

        public void UpdateStock(Dictionary<string, int> purchaseQuantities)
        {
            foreach (var (type, quantity) in purchaseQuantities)
            {
                var coffee = _coffees.FirstOrDefault(c => c.Type == type);
                if (coffee != null && coffee.Stock >= quantity)
                {
                    coffee.Stock -= quantity;
                }
            }
        }

        public decimal CalculateTotalCost(Dictionary<string, int> purchaseQuantities)
        {
            return purchaseQuantities.Sum(p =>
            {
                var coffee = _coffees.FirstOrDefault(c => c.Type == p.Key);
                if (coffee == null)
                {
                    throw new InvalidOperationException($"Coffee type '{p.Key}' not found.");
                }
                return coffee.Price * p.Value;
            });
        }
    }
}
