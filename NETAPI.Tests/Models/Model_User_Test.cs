using NETAPI.Models;

namespace NETAPI.Tests.Models
{
    public class Model_User_Test
    {
        [Fact]
        public void User_Initialization()
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

            // Assert
            Assert.Equal("Hung", user.FirstName);
            Assert.Equal("Nguyen", user.LastName);
            Assert.Equal("hungnguyen@gmail.com", user.Email);
            Assert.Equal("temp-hashed-password", user.Hash);
            Assert.Equal(now, user.CreatedAt);
            Assert.Equal(now, user.UpdatedAt);
        }
    }
}
