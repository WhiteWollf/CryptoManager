using System.ComponentModel.DataAnnotations;

namespace DataContext.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public IList<RoleDto> Roles { get; set; }
}

public class UserRegisterDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [MinLength(6)]
    public string PasswordConfirm { get; set; }
}

public class ChangePasswordDto
{

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string OldPassword { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [MinLength(6)]
    public string PasswordConfirm { get; set; }
}

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

public class UserUpdateDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public IList<int> RoleIds { get; set; }
}  
