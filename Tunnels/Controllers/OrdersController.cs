﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tunnels.Core.Models;
using Tunnels.Core.Services;
using Tunnels.Core.Views;
using Tunnels.DTOs.User;
using Tunnels.Validators;

namespace Tunnels.Controllers {
    /// <summary>
    /// Orders Controller
    /// </summary>
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService Orderservice, IMapper mapper) {
            _orderService = Orderservice;
            _mapper = mapper;
        }

        #region Post

        /// <summary>
        /// Create a Order
        /// </summary>
        /// <param name="createOrderRequest"></param>
        /// <returns>CreateOrderResponse</returns>
        [HttpPost("")]
        public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderRequest createOrderRequest) {
            var validator = new CreateOrderRequestValidator();
            var validationResult = await validator.ValidateAsync(createOrderRequest);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors); // TODO: needs refining on message format, error codes

            var OrderToCreate = _mapper.Map<CreateOrderRequest, Order>(createOrderRequest);

            var newOrder = await _orderService.CreateOrder(OrderToCreate);

            var OrderResource = _mapper.Map<Order, CreateOrderResponse>(newOrder);

            return Ok(OrderResource);
        }

        /// <summary>
        /// Inactivate an order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<CreateOrderResponse>> UpdateOrder([FromRoute] int orderId) {

            await _orderService.InvalidateOrder(orderId);

            return Ok(orderId);
        }

        #endregion

        #region Get
        /// <summary>
        /// GetAllOrdersWithProductsByFilterAsync
        /// </summary>
        /// <param name="ordersWithProductsFilter"></param>
        /// <returns></returns>
        [HttpPost("GetAllOrdersWithProductsByFilterAsync")]
        public async Task<ActionResult<List<OrdersWithProductsView>>> GetAllOrdersWithProductsByFilterAsync(OrdersWithProductsFilterRequest ordersWithProductsFilter) {
            var Orders = await _orderService.GetAllOrdersWithProductsByFilterAsync(ordersWithProductsFilter);
            
            return Ok(Orders);
        }

        /// <summary>
        /// GetSumOfOrders
        /// </summary>
        /// <param name="ordersWithProductsFilter"></param>
        /// <returns>Sum of Orders</returns>
        [HttpPost("GetSumOfOrders")]
        public async Task<ActionResult<double>> GetSumOfOrders(OrdersWithProductsFilterRequest ordersWithProductsFilter)
        {
            var sum = await _orderService.GetSumOfOrders(ordersWithProductsFilter);

            return Ok(sum);
        }

        ///// <summary>
        ///// Get User by Id
        ///// </summary>
        ///// <param name="id">UserId</param>
        ///// <returns>GetUserResponse</returns>
        //[HttpGet("{id}")]
        //public async Task<ActionResult<User>> GetUserById([FromRoute] int id)
        //{
        //    var user = await _userService.GetUserById(id);

        //    var getUserResponse = _mapper.Map<User, GetUserResponse>(user);

        //    return Ok(getUserResponse);
        //}      

        #endregion
    }
}
