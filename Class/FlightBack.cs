namespace Aeropuerto.Controler;
public class FlightBack{
    public required int IDFlight { get ; set ; }
    public required string Origin { get ; set ; }
    public required string Destinity { get ; set ; }
    public required int Capacity { get ; set ; }
    public required string dateTime{ get ; set ; }
    public required string departureTime { get ; set ; }
    public required string ArriveTime {get ; set ; }
    public required float Price { get ; set ;}
}