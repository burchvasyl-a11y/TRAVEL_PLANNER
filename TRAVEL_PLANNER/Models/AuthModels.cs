using System;

namespace TRAVEL_PLANNER.Models
{
    public class UserAccount
    {
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }

    public class AppSession
    {
        public string UserEmail { get; set; }
    }
}