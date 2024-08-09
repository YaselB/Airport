namespace Aeropuerto.Controler;

public interface ISendEmail {
    public string SendEmailAsync(string Email , string ConfirmationLink);
    public string SendEmailTokenAsync(string Email );
}