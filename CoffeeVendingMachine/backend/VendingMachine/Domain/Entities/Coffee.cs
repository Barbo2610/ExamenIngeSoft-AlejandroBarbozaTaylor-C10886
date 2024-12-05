namespace VendingMachine.Domain.Entities
{
    public class Coffee
    {
        public string Type { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}