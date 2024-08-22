using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class Agent
    {
        [Key]
        public int? Id { get; set; }

        public string Nickname { get; set; }
        public string PhotoUrl { get; set; }
  

        [Range(1, 1001, ErrorMessage = "The value must be greater than 1 or smaller then 1000")]
        public Location? Location { get; set; }

        public string? Status { get; set; }

    }
}
