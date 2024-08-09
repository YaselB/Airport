using System.ComponentModel.DataAnnotations;
namespace Aeropuerto.Controler;

public class Flight {
    [Key]
    public int IDFlight { get ; set ;}
    public required string Origin { get ;set ;}
    public required string Destinity { get ; set ;}
    public required int Capacity { get ; set ;}
    public required DateTime dateTime{ get ; set ; }
    public required string departureTime { get ; set ; }
    public required string ArriveTime {get ; set ; }
    public required ICollection<Reservation>? Freservations{ get ; set ; }
    public required ICollection<Seat>? Seats { get ; set ;}
    public required float Price { get ; set ;}

}