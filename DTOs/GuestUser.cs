namespace GuestSystemBack.DTOs
{
    public class GuestUser
    {
        //public string id { get; set; }
        public string name { get; set; }
        public string guestType { get; set; }
        public string status { get; set; }
        public string personBeingVisited { get; set; }
        public GuestInfo guestInfo { get; set; }
        public GuestAccessInfo guestAccessInfo { get; set; }
        public string portalId { get; set; }
    }
}
