namespace Aeropuerto.Controler;

public class NewFlight {
    public required string AdminEmail { get ; set ; }
    public required string Origin { get ; set ; }
    public required string Destinity { get ; set ; }
    public required int Capacity { get ; set ; }
    public required string dateTime { get ; set ;}
    public required string DepartureTime { get ; set ; }
    public required string ArriveTime { get ; set ; }
    public required float Price { get ; set ;}
}