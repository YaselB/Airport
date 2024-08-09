using System.ComponentModel.DataAnnotations;

namespace Aeropuerto.Controler;

public class Role {
   [Key]
   public int IDRole { get; set; }
   public required string Rol { get ; set ;}
   public required UserModel? userModel {get ; set ;}
   public required string? UserCI {get ; set ;}
}