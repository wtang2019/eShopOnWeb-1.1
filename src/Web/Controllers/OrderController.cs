﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.Web.Features.MyOrders;
using Microsoft.eShopWeb.Web.Features.OrderDetails;
using static NewRelic.Api.Agent.NewRelic;

namespace Microsoft.eShopWeb.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize] // Controllers that mainly require Authorization still use Controller/View; other pages use Pages
[Route("[controller]/[action]")]
public class OrderController : Controller
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {
        var viewModel = await _mediator.Send(new GetMyOrders(User.Identity.Name));

        return View(viewModel);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> Detail(int orderId)
    {
        var viewModel = await _mediator.Send(new GetOrderDetails(User.Identity.Name, orderId));

        if (viewModel == null)
        {
            try
        {
            throw new Exception("500 Internal Server Error");
        }
        catch(Exception ex)
        {
                NewRelic.Api.Agent.NewRelic.NoticeError(ex);
        }
        }

        return View(viewModel);
    }
}
