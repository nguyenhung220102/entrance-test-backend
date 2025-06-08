using NETAPI.Models;

namespace NETAPI.Tests.Models
{
    public class Model_Token_Test
    {
        [Fact]
        public void Token_Initialization()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var token = new Token
            {
                Id = 1,
                UserId = 1,
                RefreshToken = "temp-refresh-token",
                ExpiresIn = now.AddDays(30).ToString("o"),
                CreatedAt = now,
                UpdatedAt = now
            };

            // Assert
            Assert.Equal(1, token.Id);
            Assert.Equal(1, token.UserId);
            Assert.Equal("temp-refresh-token", token.RefreshToken);
            Assert.Equal(now.ToString("o"), token.CreatedAt.ToString("o"));
            Assert.Equal(now.ToString("o"), token.UpdatedAt.ToString("o"));
        }

        [Fact]
        public void Token_User_Relation()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var user = new User
            {
                FirstName = "Hung",
                LastName = "Nguyen",
                Email = "hungnguyen@gmail.com",
                Hash = "temp-hashed-password",
                CreatedAt = now,
                UpdatedAt = now
            };

            var token = new Token
            {
                UserId = user.Id,
                User = user
            };

            // Assert
            Assert.Equal(user.Id, token.UserId);
            Assert.Equal("hungnguyen@gmail.com", token.User.Email);
        }
    }
}
