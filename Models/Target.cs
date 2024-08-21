using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class Target
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Job { get; set; }
        [Range(0, 1001, ErrorMessage = "The value must be greater than 0 or smaller then 1000")]
        public Location Location { get; set; }
       
    }
}
