using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETAPI.DTOs;
using NETAPI.Models;
using NETAPI.Repository;
using NETAPI.Services;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _repository;
    private readonly JwtService _jwtService;

    public AuthController(IAuthRepository repository, JwtService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpDTO dto)
    {
        try
        {
            // Check email's existence
            var existingUser = await _repository.GetUserByEmailAsync(dto.Email);
            if  (existingUser != null)
                return BadRequest("Email is already registered.");

            // Check email format
            if (string.IsNullOrWhiteSpace(dto.Email) || !CheckValidEmail(dto.Email))
                return BadRequest("Invalid email format.");

            // Check password's length
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8 || dto.Password.Length > 20)
                return BadRequest("Password must be between 8 and 20 characters.");

            // Encrypting password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Hash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddUserAsync(user);
            await _repository.SaveChangesAsync();

            // Response data
            var response = new
            {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                displayName = $"{user.FirstName} {user.LastName}"
            };

            // Sign up successfully
            return Created("", response); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal error occurred.");
        }
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInDTO dto)
    {
        try
        {
            // Check email format
            if (string.IsNullOrWhiteSpace(dto.Email) || !CheckValidEmail(dto.Email))
                return BadRequest("Invalid email format.");

            // Check password's length
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8 || dto.Password.Length > 20)
                return BadRequest("Password must be between 8 and 20 characters.");

            // Check valid email and valid password
            var user = await _repository.GetUserByEmailAsync(dto.Email);
            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Hash);
            if (!isValid)
                return BadRequest("Invalid email or password.");

            // Generate JWT tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Create refresh token
            var token = new Token
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddDays(30).ToString("o"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Saving refresh token 
            await _repository.AddRefreshTokenAsync(token);
            await _repository.SaveChangesAsync();

            var response = new
            {
                user = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    displayName = $"{user.FirstName} {user.LastName}"
                },
                token = accessToken,
                refreshToken = refreshToken
            };

            // Sign In successfully
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal error occurred.");
        }
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOut(RefreshTokenDTO dto)
    {
        try
        {
            // Check refresh token's existence
            var refreshToken = await _repository.GetRefreshTokenAsync(dto.RefreshToken);

            await _repository.RemoveAllRefreshTokensAsync(refreshToken);
            await _repository.SaveChangesAsync();

            // Signing out successfully
            return NoContent(); 
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error occurred."); 
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDTO dto)
    {
        try
        {
            // Check refresh token's existence 
            var oldToken = await _repository.GetRefreshTokenAsync(dto.RefreshToken);
            if (oldToken == null)
                return NotFound("Refresh token not found.");

            // Get user information
            var user = await _repository.GetUserByIdAsync(oldToken.UserId);
            if (user == null)
                return StatusCode(500, "User not found."); 

            // Remove previous refresh token
            await _repository.RemoveRefreshTokenAsync(oldToken);

            // Create new token
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var accessToken = _jwtService.GenerateAccessToken(user);

            var newToken = new Token
            {
                UserId = user.Id,
                RefreshToken = newRefreshToken,
                ExpiresIn = DateTime.UtcNow.AddDays(30).ToString("o"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddRefreshTokenAsync(newToken);
            await _repository.SaveChangesAsync();

            // Create new tokens successfully
            return Ok(new
            {
                token = accessToken,
                refreshToken = newRefreshToken
            }); 
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error occurred."); 
        }
    }
    private bool CheckValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}