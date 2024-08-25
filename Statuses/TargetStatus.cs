using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Statuses
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
