using VendingMachine.Domain.Entities;

namespace VendingMachine.Models
{
    public class PurchaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ChangeResponse Change { get; set; }
        public List<Coffee> RemainingCoffees { get; set; }
    }

    public class ChangeResponse
    {
        public decimal Total { get; set; }
        public Dictionary<int, int> Breakdown { get; set; }
    }
}