namespace Aeropuerto.Controler;
public class LoginResponse {
    public required string Token { get; set; }
    public required string Role { get; set; }
    public required bool IsAuthenticated {get ; set ;}
}