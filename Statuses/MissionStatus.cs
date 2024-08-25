using System.ComponentModel.DataAnnotations;

namespace MosadAPIServer.Statuses
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
