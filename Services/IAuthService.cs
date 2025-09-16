using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Entities;
using StudentManagementApi.Entities.Models;

namespace StudentManagementApi.Services {
    public interface IAuthService {
        Task<User?> RegisterAsync(UserRegisterDto request);
        Task<TokenResponseDto?> LoginAsync(UserLoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
