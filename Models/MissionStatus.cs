using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Models
{
    public class MissionStatus
    {
        public enum Status
        {
            Offer,
            InMission,
            Finish
        }
    }
}
