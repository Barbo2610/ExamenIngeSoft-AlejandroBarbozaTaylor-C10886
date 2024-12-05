using Microsoft.AspNetCore.Mvc;
using VendingMachine.Handlers;
using VendingMachine.Models;
using VendingMachine.Services;

namespace VendingMachine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeesController : ControllerBase
    {
        private readonly ICoffeeService _coffeeService;
        private readonly PurchaseCoffeeHandler _purchaseHandler;

        public CoffeesController(ICoffeeService coffeeService, PurchaseCoffeeHandler purchaseHandler)
        {
            _coffeeService = coffeeService;
            _purchaseHandler = purchaseHandler;
        }

        // Obtener cafés disponibles.
        [HttpGet("GetCoffees")]
        public IActionResult GetCoffees()
        {
            var coffees = _coffeeService.GetAvailableCoffees();
            return Ok(coffees);
        }

        // Comprar café.
        [HttpPost("Purchase")]
        public IActionResult Purchase([FromBody] PurchaseRequest request)
        {
            if (request.PurchaseQuantities == null || request.PaidAmount <= 0 || request.ManualCoinsInput == null)
            {
                return BadRequest(new { Error = "Solicitud inválida. Verifique los datos ingresados." });
            }

            var result = _purchaseHandler.Purchase(request.PurchaseQuantities, request.PaidAmount, request.ManualCoinsInput);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    Error = result.Message,
                    RemainingCoffees = result.RemainingCoffees
                });
            }

            return Ok(new
            {
                Message = result.Message,
                Change = result.Change,
                RemainingCoffees = result.RemainingCoffees
            });
        }
    }  
}