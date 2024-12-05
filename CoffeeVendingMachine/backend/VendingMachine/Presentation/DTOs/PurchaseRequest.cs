namespace VendingMachine.Models
{
    public class PurchaseRequest
    {
        public Dictionary<string, int> PurchaseQuantities { get; set; } // Cantidad de cafés
        public decimal PaidAmount { get; set; } // Monto total pagado
        public Dictionary<int, int> ManualCoinsInput { get; set; } // Monedas o billetes ingresados manualmente
    }
}