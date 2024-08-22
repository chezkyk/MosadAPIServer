using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }

        public Agent AgentId { get; set; }

        public Target TargetId { get; set; }

        public double? TimeLeft { get; set; }

        public double? ExecutionTime { get; set; }

        public string? Status { get; set; }


    }
}
