using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aeropuerto.Controler;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    public readonly DBContext context;
    public readonly ISendEmail _sendEmail;
    public readonly UserService _userService;
    public readonly IGeneratedJwt _generatedJwt;
    public UserController(DBContext dbContext, ISendEmail sendEmail, UserService userService, IGeneratedJwt generatedJwt)
    {
        context = dbContext;
        _sendEmail = sendEmail;
        _userService = userService;
        _generatedJwt = generatedJwt;
    }

    [HttpPost("Register")]
    public async Task<ActionResult> PostUsuariosItem([FromBody] UserRegister userRegister)
    {
        var find = await context.User.FirstOrDefaultAsync(User => User.Email == userRegister.Email);
        if (find != null)
        {
            return NotFound("Email not available");
        }
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-]+$";
        Match match = Regex.Match(userRegister.Email, pattern);
        if (!match.Success)
        {
            return NotFound("Please enter a valid email");
        }
        string pattern2 = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$";
        Match match1 = Regex.Match(userRegister.Password, pattern2);
        if (!match1.Success)
        {
            if (!Regex.IsMatch(userRegister.Password, "(?=.*[A-Z])"))
            {
                return BadRequest("The password must have at least one uppercase letter");
            }
            else if (!Regex.IsMatch(userRegister.Password, "(?=.*[a-z])"))
            {
                return BadRequest("The password must have at least one lowercase letter");
            }
            else if (!Regex.IsMatch(userRegister.Password, "(?=.*\\d)"))
            {
                return BadRequest("The password must have numbers");
            }
            else if (!Regex.IsMatch(userRegister.Password, "(?=.*[^\\da-zA-Z])"))
            {
                return BadRequest("The password must contain special characters");
            }
            else
            {
                return BadRequest("The password must have 6 or more characters");
            }
        }
        UserModel userModel = new UserModel
        {
            CI = userRegister.CI,
            Name = userRegister.Username,
            Lastname = userRegister.LastName,
            Email = userRegister.Email,
            Password = _userService.GeneratePassword(userRegister.Password),
            EmailConfirmed = false,
            Token = _sendEmail.SendEmailAsync(userRegister.Email, "http://localhost:5167/api/User/confirm?token="),
            reservations = null,
            rol = null,
        };
        Role role = new Role
        {
            Rol = "User",
            UserCI = userRegister.CI,
            userModel = null,
        };
        await context.User.AddAsync(userModel);
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
        return Ok("Please confirm your email");
    }
    [HttpPatch("ChangeRole")]
    public async Task<IActionResult> ChangeRole([FromBody] AdminWork adminWork)
    {
        var userRegister = await context.User.FirstOrDefaultAsync(r => r.Email == adminWork.AdminEmail);
        if (userRegister == null)
        {
            return NotFound("Please the AdminEmail do not registered");
        }
        var verifyRole = await context.Roles.FirstOrDefaultAsync(r => r.UserCI == userRegister.CI);
        if (verifyRole == null || verifyRole.Rol == "User")
        {
            return Forbid();
        }
        var userRegistertochange = await context.User.FirstOrDefaultAsync(r => r.Email == adminWork.EmailUser);
        if (userRegistertochange == null)
        {
            return NotFound("User do not registered");
        }
        if (!userRegister.EmailConfirmed)
        {
            return Unauthorized("Please confirm the admin email");
        }
        if (!userRegistertochange.EmailConfirmed)
        {
            return Unauthorized("Please confirm the user email");
        }
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.UserCI == userRegistertochange.CI);
        if (userRole == null)
        {
            return NotFound("User have not a role");
        }
        Console.WriteLine(adminWork.Role);
        userRole.Rol = adminWork.Role;
        Console.WriteLine(userRole.Rol);
        context.Entry(userRole).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Ok("Role Change succesfully");
    }
    [HttpPost("Autenticate")]
    public async Task<IActionResult> UserAutenticate([FromBody] UserToken userToken)
    {
        if (_generatedJwt.VerifyToken(userToken.token))
        {
            return Unauthorized("Please login again");
        }
        var find = context.User.FirstOrDefault(option => option.Token == userToken.token);
        if (find == null)
        {

            return NotFound("User not registered");
        }
        find.Token = _generatedJwt.GeneratedToken(find.Email, find.Password);
        context.Entry(find).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Ok(find.Token);
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
        var user = await context.User.FirstOrDefaultAsync(n => n.Token == token);
        if (user == null)
        {
            return NotFound("Invalid confirmation token");
        }
        user.EmailConfirmed = true;
        user.Token = "";
        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Ok("Email is confirmed");
    }
    [HttpPost("login")]
    public async Task<IActionResult> UserLogin([FromBody] NewPassword newPassword)
    {
        var user = await context.User.FirstOrDefaultAsync(option => option.Email == newPassword.Email);
        if (user == null)
        {
            return NotFound("User do not registered");
        }
        if (!user.EmailConfirmed)
        {
            return BadRequest("Please confirm your email");
        }
        if (!_userService.verifyPassword(newPassword.Password, user.Password))
        {
            return BadRequest("Wrong Password");
        }
        var role = await context.Roles.FirstOrDefaultAsync(option => option.UserCI == user.CI);
        if (role == null)
        {
            return NotFound("User haven't role");
        }
        user.Token = _generatedJwt.GeneratedToken(newPassword.Email, newPassword.Password);
        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
        LoginResponse loginResponse = new LoginResponse
        {
            Token = user.Token,
            Role = role.Rol,
            IsAuthenticated = user.EmailConfirmed
        };
        return Ok(loginResponse);
    }
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> GetTokenUser([FromBody] SendTokenUser sendTokenUser)
    {
        var find = await context.User.FirstOrDefaultAsync(option => option.Email == sendTokenUser.Email);
        if (find == null)
        {
            return NotFound("User do not registered");
        }
        if (!find.EmailConfirmed)
        {
            return BadRequest("Please confirm your Email and after Change your Password");
        }
        find.Token = _sendEmail.SendEmailTokenAsync(find.Email);
        context.Entry(find).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Ok("Please check your email");
    }

    [HttpPost("ConfirmChangeToken")]
    public async Task<IActionResult> ConfirmChangeToken([FromBody] string Token)
    {
        var userRegister = await context.User.FirstOrDefaultAsync(option => option.Token == Token);
        if (userRegister == null)
        {
            return NotFound("Please the user do not exist");
        }
        if (!userRegister.EmailConfirmed)
        {
            return BadRequest("Please confirm your email");
        }
        return Ok(userRegister.Email);
    }

    [HttpPatch("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] NewPassword newPassword)
    {
        var find = await context.User.FirstOrDefaultAsync(option => option.Email == newPassword.Email);
        if (find == null)
        {
            return NotFound("Please this email do not be registered");
        }
        if (!find.EmailConfirmed)
        {
            return BadRequest("Please confirm your email");
        }
        find.Password = _userService.GeneratePassword(newPassword.Password);
        context.Entry(find).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Ok("The password change was succesfully");
    }
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromBody] UserDelete userdelete)
    {
        var userDelete = await context.User.FirstOrDefaultAsync(r => r.Email == userdelete.EmailUser);
        if (userDelete == null)
        {
            return NotFound("user do not registered");
        }
        if (!userDelete.EmailConfirmed)
        {
            return BadRequest("Please confirm your email");
        }
        var Admin = await context.User.FirstOrDefaultAsync(r => r.Email == userdelete.AdminEmail);
        if (Admin == null)
        {
            return NotFound("Admin do not registered");
        }
        if (!Admin.EmailConfirmed)
        {
            return BadRequest("Please Admin confirm your email");
        }
        var AdminRole = await context.Roles.FirstOrDefaultAsync(r => r.UserCI == Admin.CI);
        if (AdminRole == null)
        {
            return NotFound("Please user have not Role");
        }
        if (AdminRole.Rol != "Admin")
        {
            return Forbid("You cannot perform this operation because you are not an Admin");
        }
        var reservations = context.Reservations.Where(r => r.UserCI == userDelete.CI).ToList();
        context.Reservations.RemoveRange(reservations);
        var roles = context.Roles.Where(r => r.UserCI == userDelete.CI).ToList();
        context.Roles.RemoveRange(roles);
        context.User.Remove(userDelete);
        await context.SaveChangesAsync();
        return Ok("User delete succesfully");
    }
    [Authorize]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUser([FromQuery] string Email)
    {
        var user = await context.User.FirstOrDefaultAsync(option => option.Email == Email);
        if (user == null)
        {
            return NotFound("User do not registered");
        }
        var role = await context.Roles.FirstOrDefaultAsync(option => option.UserCI == user.CI);
        if (role == null)
        {
            return NotFound("User haven't role");
        }
        if (role.Rol != "Admin")
        {
            return Forbid("You cannot perform this operation because you are not an Admin");
        }

        var users = context.User.ToList();
        if (users == null || !users.Any())
        {
            return NotFound("Not users registered");
        }
        var roles = context.Roles.ToList();
        List<UserBack> userBacks = new List<UserBack>();

        foreach (var i in users)
        {
            foreach (var j in roles)
            {
                if (i.CI == j.UserCI)
                {
                    UserBack userBack = new UserBack
                    {
                        CI = i.CI,
                        Email = i.Email,
                        Username = i.Name,
                        LastName = i.Lastname,
                        Role = j.Rol
                    };
                    userBacks.Add(userBack);
                    break;
                }
            }

        }
        return Ok(userBacks);
    }




}


