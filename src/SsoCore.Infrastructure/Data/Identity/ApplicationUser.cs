using Microsoft.AspNetCore.Identity;

namespace SsoCore.Infrastructure.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? LastName { get; set;  }
        public string? FirstName { get; set;  }
        public string? MiddleNames { get; set;  }
        public bool IsDisabled { get; set;  }
        public string CreatedBy { get; set;  } = "Unknown";
        public DateTime CreatedAt { get; set;  }
        public string? LastUpdatedBy { get; set;  }
        public DateTime? LastUpdatedAt { get; set;  }

        public ICollection<IdentityUserClaim<string>> UserClaims { get; set;  } = [];
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

        public static ApplicationUser Create
        (
            string email, 
            string firstName,
            string lastName,
            string createdBy,
            string? middleNames = null,
            string? phoneNumber = null,
            bool emailConfirmed = false
        )
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                MiddleNames = middleNames,
                PhoneNumber = phoneNumber,
                EmailConfirmed = emailConfirmed
            };
            
            user.Enable2Fa();
            user.SetCreatedBy(createdBy);

            return user;
        }
        
        public void Enable2Fa()
        {
            this.TwoFactorEnabled = true;
        }

        public void SetCreatedBy(string updatedBy)
        {
            this.CreatedAt = DateTime.UtcNow;
            this.CreatedBy = updatedBy;
        }

        public void SetLastUpdatedBy(string updatedBy)
        {
            this.LastUpdatedAt = DateTime.UtcNow;
            this.LastUpdatedBy = updatedBy;
        }

        public void Update(string? firstName, string? lastName, string? middleNames, string updatedBy)
        {
            FirstName = firstName ?? FirstName;
            LastName = lastName ?? LastName;
            MiddleNames = middleNames ?? MiddleNames;
            SetLastUpdatedBy(updatedBy);
        }
    }
}
