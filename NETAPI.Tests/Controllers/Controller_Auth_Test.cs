using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NETAPI.DTOs;
using NETAPI.Models;
using NETAPI.Repository;
using NETAPI.Services;

namespace NETAPI.Tests.Controllers
{
    public class Controller_Auth_Test
    {
        private readonly Mock<IAuthRepository> _repositoryMock;
        private readonly Mock<JwtService> _jwtServiceMock;
        private readonly AuthController _controller;
        public Controller_Auth_Test()
        {
            _repositoryMock = new Mock<IAuthRepository>();
            _jwtServiceMock = new Mock<JwtService>(Mock.Of<IConfiguration>());
            _jwtServiceMock.Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
                          .Returns("access-token");
            _jwtServiceMock.Setup(s => s.GenerateRefreshToken())
                          .Returns("refresh-token");
            _controller = new AuthController(_repositoryMock.Object, _jwtServiceMock.Object);
        }
        [Fact]
        public async Task SignIn_InvalidEmail_BadRequest()
        {
            // Arrange
            var dto = new SignInDTO { Email = "test.com", Password = "12345678" };

            // Act
            var result = await _controller.SignIn(dto);

            // Assert
            var res = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid email format.", res.Value);
        }

        [Fact]
        public async Task SignIn_InvalidPasswordUnder8_BadRequest()
        {
            // Arrange
            var dto = new SignInDTO { Email = "test@gmail.com", Password = "1234567" };

            // Act
            var result = await _controller.SignIn(dto);

            // Assert
            var res = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Password must be between 8 and 20 characters.", res.Value);
        }

        [Fact]
        public async Task SignIn_InvalidPasswordExeeds20_BadRequest()
        {
            // Arrange
            var dto = new SignInDTO { Email = "test@gmail.com", Password = "123456712345671234567" };

            // Act
            var result = await _controller.SignIn(dto);

            // Assert
            var res = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Password must be between 8 and 20 characters.", res.Value);
        }

        [Fact]
        public async Task SignIn_InvalidCredential_BadRequest()
        {
            // Arrange
            var dto = new SignInDTO { Email = "test@gmail.com", Password = "wrong-pass" };
            var user = new User
            {
                Id = 1,
                Email = dto.Email,
                FirstName = "Nguyen",
                LastName = "Hung",
                Hash = BCrypt.Net.BCrypt.HashPassword("correct-pass")
            };

            _repositoryMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act
            var result = await _controller.SignIn(dto);

            // Assert
            var res = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid email or password.", res.Value);
        }

        [Fact]
        public async Task SignIn_ValidCredential_OK()
        {
            // Arrange
            var dto = new SignInDTO { Email = "test@gmail.com", Password = "wrong-pass" };
            var user = new User
            {
                Id = 1,
                Email = dto.Email,
                FirstName = "Nguyen",
                LastName = "Hung",
                Hash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _repositoryMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act
            var result = await _controller.SignIn(dto);

            // Assert
            var res = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(res); //TODO
        }
    }
}
