using System.Collections.Generic;

namespace VendingMachine.Domain.Entities
{
    public class Change
    {
        public decimal Total { get; set; }
        public Dictionary<int, int> Breakdown { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}