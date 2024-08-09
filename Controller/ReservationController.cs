using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Aeropuerto.Controler;

public class ReservationController : ControllerBase
{
    private readonly DBContext context;
    public ReservationController(DBContext dbContext)
    {
        context = dbContext;
    }
    [HttpPost("CreateReservation")]
    public async Task<IActionResult> CreateReservation([FromBody] NewReservation newReservation)
    {
        var user = await context.User.FirstOrDefaultAsync( option => option.Email == newReservation.Email );
        if(user == null){
            return NotFound("User not have registered");
        }
        var  flight = await context.Flights.FirstOrDefaultAsync( option => option.IDFlight == newReservation.IDFlight );
        if(flight == null){
            return NotFound("Flight not have registered ");
        }
        var uservalid = await context.Reservations.FirstOrDefaultAsync(option => option.UserCI == user.CI && option.FlightID == newReservation.IDFlight);
        if (uservalid == null)
        {
            var find = context.Seats.Where(s => s.IDFlight == newReservation.IDFlight).OrderBy(s => s.seatNumber).ToList();
            if (!find.Any())
            {
                Seat seat = new Seat
                {
                    IDFlight = newReservation.IDFlight,
                    seatNumber = 1,
                    flight = null,
                };
                Reservation reservation = new Reservation
                {
                    flight = null,
                    FlightID = newReservation.IDFlight,
                    seatNumber = 1,
                    UserCI = user.CI,
                    userModel = null,
                };
                await context.Seats.AddAsync(seat);
                await context.Reservations.AddAsync(reservation);
                await context.SaveChangesAsync();
                return Ok("Reservation was created succesfully");
            }
            for (int i = 0; i < find.Count; i++)
            {
                if (find[i].seatNumber != i + 1)
                {
                    Seat seat = new Seat
                    {
                        flight = null,
                        IDFlight = newReservation.IDFlight,
                        seatNumber = i + 1
                    };
                    Reservation reservation = new Reservation
                    {
                        flight = null,
                        FlightID = newReservation.IDFlight,
                        seatNumber = i + 1,
                        UserCI = user.CI,
                        userModel = null,
                    };
                    await context.Seats.AddAsync(seat);
                    await context.Reservations.AddAsync(reservation);
                    await context.SaveChangesAsync();
                    return Ok("Reservation was created succesfully");
                }
            }
        }
        return BadRequest("User already has a reservation on this flight");
    }
    [HttpGet("GetAll/{email}")]
    public async Task<IActionResult> GetReservation(string email)
    {
        var user = await context.User.FirstOrDefaultAsync(option => option.Email == email);
        if (user == null)
        {
            return NotFound("User do not registered");
        }
        var reservation = context.Reservations.Where(option => option.UserCI == user.CI).ToList();
        if (reservation == null)
        {
            return BadRequest("The user haven't reservations");
        }
        List<ShowReservation> showReservations = new List<ShowReservation>();
        foreach (var reserv in reservation)
        {
            var flight = await context.Flights.FirstOrDefaultAsync(option => option.IDFlight == reserv.FlightID);
            if (flight == null)
            {
                return NotFound("Flight not registered");
            }
            ShowReservation show = new ShowReservation
            {
                ArriveTime = flight.ArriveTime,
                DateTime = flight.dateTime,
                DepartureTime = flight.departureTime,
                Destinity = flight.Destinity,
                Origin = flight.Origin,
                Price = flight.Price,
                Seat = reserv.seatNumber,
                ID = reserv.IdReservation
            };
            showReservations.Add(show);
        }
        return Ok(showReservations);
    }
    [HttpDelete("RemoveReservation/{id}")]
    public async Task<IActionResult> DeleteReservation( int id)
    {
        var reservation = await context.Reservations.FirstOrDefaultAsync(option => option.IdReservation == id);
        if (reservation == null)
        {
            return NotFound("The reservation does not exist");
        }
        var seat = await context.Seats.FirstOrDefaultAsync(option => option.seatNumber == reservation.seatNumber && option.IDFlight == reservation.FlightID);
        if(seat != null){
            context.Seats.Remove(seat);
            await context.SaveChangesAsync();
        }
        context.Reservations.Remove(reservation);
        await context.SaveChangesAsync();
        return Ok("Reservation deleted succesfully");
    }
}