using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagementApi.Data;
using StudentManagementApi.Entities;
using StudentManagementApi.Entities.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudentManagementApi.Services {
    public class AuthService : IAuthService {

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config) {
            _context = context;
            _config = config;
        }

        public async Task<User?> RegisterAsync(UserRegisterDto request) {

            if(await _context.Users.AnyAsync(u => u.Username == request.Username))
                return null;

            var user = new User { Username = request.Username, Role = request.Role };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request) {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if(user is null)
                return null;

            var check = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if(check == PasswordVerificationResult.Failed)
                return null;

            return new TokenResponseDto {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request) {

            var user = await _context.Users.FindAsync(request.UserId);

            if(user == null)
                return null;

            if(user.RefreshToken != request.RefreshToken)
                return null;

            if(user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            return new TokenResponseDto {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private string CreateToken(User user) {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AppSettings:Token"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _config["AppSettings:Issuer"],
                audience: _config["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), 
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken() {

            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user) {

            var refresh = GenerateRefreshToken();
            user.RefreshToken = refresh;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return refresh;
        }
    }
}
