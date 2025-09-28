using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskifyAPI.Data;
using TaskifyAPI.Models;
using TaskifyAPI.DTOs;
using System.Linq.Expressions;
namespace TaskifyAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto dto);
        Task<User> Register (RegisterDto dto);
        Task<User?> GetUserByName(string username);
    }
    public class AuthService : IAuthService
    {
        private readonly AppDbContext? _context;
        private readonly IConfiguration? _config;
        public AuthService(AppDbContext? context, IConfiguration? config)
        {
            _context = context;
            _config = config;
        }
        public async Task<User?> GetUserByName(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        public async Task<User> Register(RegisterDto dto)
        {
            CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);
            var user = new User { UserName = dto.UserName,PasswordHash = hash,PasswordSalt = salt};
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>u.UserName == dto.UserName);
            if (user == null) throw new Exception("Kullanıcı bulunamadı");
            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Şifre yanlış");

            //create token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDecriptor);
            return new AuthResponseDto { Token = tokenHandler.WriteToken(token) };
        }
        //password helpers
        private void CreatePasswordHash(string password,out byte[] hash,out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(storedHash);
        }
    }
}
