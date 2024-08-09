using System.ComponentModel.DataAnnotations;

namespace Aeropuerto.Controler;

public class Seat {
    [Key]
    public int IDSeat { get; set; }
    public required int seatNumber { get; set; }
    public required Flight? flight { get; set; }
    public required int IDFlight { get ; set ;}
    
}