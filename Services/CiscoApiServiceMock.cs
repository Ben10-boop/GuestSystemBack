using GuestSystemBack.DTOs;
using GuestSystemBack.Interfaces;

namespace GuestSystemBack.Services
{
    public class CiscoApiServiceMock : ICiscoApiService
    {
        public List<GuestUser> GetCurrentWifiUsers()
        {
            List<GuestUser> users = new()
            {
                new GuestUser() {
                    name = "Justinas Justinauskas",
                    guestType = "Regular",
                    status = "ACTIVE",
                    personBeingVisited = "Anupras Anupraitis",
                    portalId = "0001223",
                    guestInfo = new GuestInfo()
                    {
                        userName = "JJfdasgd",
                        password = "sdasgajpewj123",
                        emailAddress = "JJustinauskas@gmail.com",
                    },
                    guestAccessInfo = new GuestAccessInfo()
                    {
                        validDays = 1,
                        fromDate = "04/25/2023 12:30",
                        toDate = "04/26/2023 12:30"
                    }
                },
                new GuestUser() {
                    name = "Petras Petrauskas",
                    guestType = "Regular",
                    status = "ACTIVE",
                    personBeingVisited = "Anupras Anupraitis",
                    portalId = "0001223",
                    guestInfo = new GuestInfo()
                    {
                        userName = "vsdvsvdsDSS",
                        password = "dsvds126sgsf",
                        emailAddress = "AAnuprauskas@gmail.com",
                    },
                    guestAccessInfo = new GuestAccessInfo()
                    {
                        validDays = 1,
                        fromDate = "04/25/2023 10:44",
                        toDate = "04/26/2023 10:44"
                    }
                }
            };
            return users;
        }

        public void PostWifiUser(GuestUserDTO guestUser)
        {
            
        }
    }
}
