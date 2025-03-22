namespace Hoi4Strats.Controllers
{
    public interface ITokenService
    {
        string GenerateToken(string userId, List<string> roles);
    }
}
