using System.ComponentModel.DataAnnotations;


namespace Aeropuerto.Controler;

public class UserModel 
{
[Key]
public required string CI { get ; set ;}
public required string Email { get ; set ;}
public required string Password {get ; set ;}
public required bool EmailConfirmed { get ; set ;}
public required string Name { get ; set ;}
public required string Lastname { get; set;}
public required string Token { get ; set ;}
public required ICollection<Reservation>? reservations{ get ; set ; }
public required Role? rol{ get ; set ; }
}