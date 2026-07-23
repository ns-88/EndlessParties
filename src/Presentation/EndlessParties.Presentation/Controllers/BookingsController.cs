using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Application.Abstractions.Bookings.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Presentation.Controllers;

/// <summary>
/// Контроллер для работы с бронированиями
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    /// <summary>
    /// Сервис <see cref="IBookingService"/>
    /// </summary>
    private readonly IBookingService _bookingService;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }


    /// <summary>
    /// Получение бронирования по идентификатору
    /// </summary>
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var bookingModel = await _bookingService.GetById(id, cancellationToken);

        return Ok(bookingModel);
    }
}