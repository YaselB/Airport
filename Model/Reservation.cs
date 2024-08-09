using System.ComponentModel.DataAnnotations;

namespace Aeropuerto.Controler;

public class Reservation {
    [Key]
    public int IdReservation { get; set; }
    public required int seatNumber { get; set; }
    public required UserModel? userModel {get ; set ;}
    public required string UserCI {get ; set ;}
    public required Flight? flight { get; set; }
    public required int FlightID { get ; set ;}
    

}