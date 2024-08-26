using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class UserDetails
    {
        [Key]
        public int? Id { get; set; }
        public string Username { get; set; }
    }
}
