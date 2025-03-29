using AutoMapper;
using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserRegisterDto userDto);
        Task<string> LoginAsync(UserLoginDto userDto);
        Task<UserDto> UpdateProfileAsync(int userId, UserUpdateDto userDto);
        Task<IList<RoleDto>> GetRolesAsync();
        Task<UserDto> GetUserAsync(int userId);
        Task<UserDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task DeleteUserAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(
            CryptoDbContext context,
            IMapper mapper,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserDto> RegisterAsync(UserRegisterDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            if(userDto.Password != userDto.PasswordConfirm)
            {
                throw new DataException("Passwords do not match.");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.Roles = new List<Role>();

            if (userDto.RoleIds != null)
            {
                foreach (var roleId in userDto.RoleIds)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                    if (existingRole != null)
                    {
                        user.Roles.Add(existingRole);
                    }
                }
            }

            if (!user.Roles.Any())
            {
                user.Roles.Add(await GetDefaultRoleAsync());
            }
            
            await _context.Users.AddAsync(user);

            //Új wallet létrehozása a regisztrált felhasználónak
            Console.WriteLine($"Ellenőrzés UserService regisztráció, Wallet létrehozása: ez itt egy id: -> {user.Id} <- ha igen, akkor át kell írni egy sort, nem kell újból lekérni, hogy az id-ja meglegyen");
            var addedUser = _context.Users.First(u => u.Email == user.Email);
            var wallet = new Wallet { UserId = addedUser.Id, Balance = 10000 };
            await _context.Wallets.AddAsync(wallet);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        private async Task<Role> GetDefaultRoleAsync()
        {
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (customerRole == null)
            {
                customerRole = new Role { Name = "User" };
                await _context.Roles.AddAsync(customerRole);
                await _context.SaveChangesAsync();
            }
            return customerRole;
        }

        public async Task<string> LoginAsync(UserLoginDto userDto)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(x => x.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return await GenerateToken(user);
        }

        private async Task<string> GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var id = await GetClaimsIdentity(user);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], id.Claims, expires: expires, signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString(CultureInfo.InvariantCulture))
            };

            if (user.Roles != null && user.Roles.Any())
            {
                claims.AddRange(user.Roles.Select(role => new Claim("roleIds", Convert.ToString(role.Id))));
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
            }

            return new ClaimsIdentity(claims, "Token");
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UserUpdateDto userDto)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _mapper.Map(userDto, user);

            if (userDto.RoleIds != null && userDto.RoleIds.Any())
            {
                user.Roles.Clear();

                foreach (var roleId in userDto.RoleIds)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                    if (existingRole != null)
                    {
                        user.Roles.Add(existingRole);
                    }
                }
            }

            if (!user.Roles.Any())
            {
                user.Roles.Add(await GetDefaultRoleAsync());
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IList<RoleDto>> GetRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return _mapper.Map<IList<RoleDto>>(roles);
        }

        public async Task<UserDto> GetUserAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);
            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == changePasswordDto.Email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (changePasswordDto.Password != changePasswordDto.PasswordConfirm)
            {
                throw new DataException("Passwords do not match.");
            }
            if(!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.Password);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }
    }
}
