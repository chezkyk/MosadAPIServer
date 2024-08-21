using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }

        public int AgentId { get; set; }

        public int TargetId { get; set; }

        public float? TimeLeft { get; set; }

        public string? ExecutionTime { get; set; }

        public MissionStatus? Status { get; set; }


    }
}
