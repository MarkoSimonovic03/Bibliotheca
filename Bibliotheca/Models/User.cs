using Microsoft.AspNetCore.Identity;

namespace Bibliotheca.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Adress {  get; set; }
    }
}
