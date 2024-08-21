using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class TargetStatus
    {
        public enum Status
        {
            Alive,
            Dead
        }
    }
}
