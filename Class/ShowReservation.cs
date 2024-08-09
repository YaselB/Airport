namespace Aeropuerto.Controler;

public class ShowReservation {
    public required int ID { get ; set ;}
    public required string Origin { get; set; }
    public required string Destinity { get ; set ;}
    public required DateTime DateTime { get; set; }
    public required string DepartureTime { get; set; }
    public required string ArriveTime { get; set; }
    public required int Seat { get; set; }
    public required float Price { get; set; }
}