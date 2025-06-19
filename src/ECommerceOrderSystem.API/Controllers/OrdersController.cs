using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ECommerceOrderSystem.Application.Orders.Commands.CreateOrder;
using ECommerceOrderSystem.Application.Orders.Queries.GetUserOrders;

namespace ECommerceOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="command">Order details</param>
        /// <returns>Created order information</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString();
                using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
                {
                    _logger.LogInformation("Creating order for UserId: {UserId}", command.UserId);

                    var result = await _mediator.Send(command);

                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning("Order creation failed: {Error}", result.Error);
                        return BadRequest(new { error = result.Error });
                    }

                    _logger.LogInformation("Order created successfully. OrderId: {OrderId}", result.Data!.OrderId);
                    return CreatedAtAction(nameof(GetUserOrders), new { userId = command.UserId }, result.Data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating order");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        /// <summary>
        /// Get orders for a specific user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>List of user orders</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(IEnumerable<UserOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString();
                using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
                {
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return BadRequest(new { error = "User ID is required" });
                    }

                    _logger.LogInformation("Retrieving orders for UserId: {UserId}", userId);

                    var query = new GetUserOrdersQuery { UserId = userId };
                    var result = await _mediator.Send(query);

                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning("Failed to retrieve orders: {Error}", result.Error);
                        return BadRequest(new { error = result.Error });
                    }

                    var orders = result.Data!.ToList();
                    _logger.LogInformation("Retrieved {Count} orders for UserId: {UserId}", orders.Count, userId);

                    return Ok(orders);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving orders for UserId: {UserId}", userId);
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }
    }
}