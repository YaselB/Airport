using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Aeropuerto.Controler;

[ApiController]
[Route("api/[controller]")]

public class FlightController : ControllerBase
{
    private readonly DBContext context;
    public FlightController(DBContext dbContext)
    {
        context = dbContext;

    }
    [Authorize]
    [HttpPost("CreateFlight")]
    public async Task<IActionResult> CreateFlight([FromBody] NewFlight newFlight)
    {
        var admin = await context.User.FirstOrDefaultAsync(x => x.Email == newFlight.AdminEmail);
        if (admin == null)
        {
            return NotFound("User do not registered");
        }
        var role = await context.Roles.FirstOrDefaultAsync(option => option.UserCI == admin.CI);
        if (role == null)
        {
            return NotFound("User do not registered");
        }
        if (role.Rol == "User")
        {
            return Forbid("you do not have permission to perform that action");
        }
        DateTime UserDate = DateTime.ParseExact(newFlight.dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Timenow = DateTime.Now;
                DateTime fullDate = new DateTime(UserDate.Year, UserDate.Month, UserDate.Day, Timenow.Hour, Timenow.Minute, Timenow.Second);
                fullDate = fullDate.ToUniversalTime();
        Flight flight = new Flight{
            Origin = newFlight.Origin,
            Destinity = newFlight.Destinity,
            Capacity = newFlight.Capacity,
            ArriveTime = newFlight.ArriveTime,
            dateTime = fullDate,
            departureTime = newFlight.DepartureTime,
            Freservations = null,
            Seats = null,
            Price = newFlight.Price,
        };
        await context.AddAsync(flight);
        await context.SaveChangesAsync();
        return Ok("Flight successfully registered");
    }
    [Authorize]
    [HttpPatch("UpdateFlights")]
    public async Task<IActionResult> UpdateFlight([FromBody] UpdateFlight updateFlight)
    {
        var admin = await context.User.FirstOrDefaultAsync(x => x.Email == updateFlight.AdminEmail);
        if (admin == null)
        {
            return NotFound("User do not registered");
        }
        var role = await context.Roles.FirstOrDefaultAsync(option => option.UserCI == admin.CI);
        if (role == null)
        {
            return NotFound("User do not registered");
        }
        if (role.Rol == "User")
        {
            return Forbid("you do not have permission to perform that action");
        }
        var flight = await context.Flights.FirstOrDefaultAsync(x => x.IDFlight == updateFlight.IDFlight);
        if (flight == null)
        {
            return NotFound("The flight does not registered");
        }
        DateTime UserDate = DateTime.ParseExact(updateFlight.dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Timenow = DateTime.Now;
                DateTime fullDate = new DateTime(UserDate.Year, UserDate.Month, UserDate.Day, Timenow.Hour, Timenow.Minute, Timenow.Second);
                fullDate = fullDate.ToUniversalTime();
        flight.Capacity = updateFlight.Capacity;
        flight.Seats = null;
        flight.Freservations = null;
        flight.Origin = updateFlight.Origin;
        flight.Destinity = updateFlight.Destinity;
        flight.dateTime = fullDate;
        flight.ArriveTime = updateFlight.ArriveTime;
        flight.departureTime = updateFlight.departureTime;
        flight.Price = updateFlight.Price;
        context.Entry(flight).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Ok("Flight changed succesfully");
    }
    
    [HttpPost("GetAll")]
    public IActionResult GetFlights()
    {
        List<Flight> flights = context.Flights.ToList();
        if (flights == null)
        {
            return NotFound("no flights available here");
        }
        List<FlightBack> newFlights = new List<FlightBack>();
        foreach (var flight in flights)
        {
            if (flight.Capacity > 0)
            {
            FlightBack newFlight = new FlightBack{
                Capacity = flight.Capacity,
                ArriveTime = flight.ArriveTime,
                dateTime = flight.dateTime.ToString("yyyy-MM-dd"),
                departureTime = flight.departureTime,
                Destinity = flight.Destinity,
                IDFlight = flight.IDFlight,
                Origin = flight.Origin,
                Price = flight.Price
            };
            newFlights.Add(newFlight);
            }
        }
        if(newFlights == null){
            return NotFound("Not flight available");
        }
        return Ok(newFlights);
    }
    [Authorize]
    [HttpDelete("DeleteFlight")]
    public async Task<IActionResult> DeleteFlight(DeleteFlight deleteFlight)
    {
        var admin = await context.User.FirstOrDefaultAsync(option => option.Email == deleteFlight.AdminEmail);
        if (admin == null)
        {
            return NotFound("User not registered");
        }
        if (!admin.EmailConfirmed)
        {
            return BadRequest("Please confirm your Email");
        }
        var role = await context.Roles.FirstOrDefaultAsync(option => option.UserCI == admin.CI);
        if (role == null)
        {
            return NotFound("User haven't role");
        }
        if (role.Rol != "Admin")
        {
            return Forbid("you do not have permission to perform that action");
        }
        var flight = await context.Flights.FirstOrDefaultAsync(option => option.IDFlight == deleteFlight.IDFlight);
        if (flight == null)
        {
            return NotFound("Flight not registered");
        }
        context.Flights.Remove(flight);
        await context.SaveChangesAsync();
        return Ok("Flight deleted succesfully");
    }

}