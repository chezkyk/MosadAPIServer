using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }

        public Agent AgentId { get; set; }

        public Target TargetId { get; set; }

        public float? TimeLeft { get; set; }

        public string? ExecutionTime { get; set; }

        public MissionStatus? Status { get; set; }


    }
}
