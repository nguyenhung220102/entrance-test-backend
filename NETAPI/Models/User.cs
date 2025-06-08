using Microsoft.AspNetCore.Identity;

namespace NETAPI.Models
{
    public class User //: IdentityUser<int>
    {
        public int Id { get; set; }
        public string Email { get; set; }  
        public string FirstName { get; set; }  
        public string LastName { get; set; }   
        public string Hash { get; set; }     
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Token> Tokens { get; set; }
    }
}