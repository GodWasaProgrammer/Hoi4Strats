using Microsoft.AspNetCore.Identity;

namespace SharedProj;

// Add profile data for application users by adding properties to the ApplicationUser class
#pragma warning disable S2094 // Classes should not be empty
public class ApplicationUser : IdentityUser
#pragma warning restore S2094 // Classes should not be empty but this is a scaffolding clas for Identity
{
}
