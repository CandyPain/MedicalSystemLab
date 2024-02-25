using MedLab3.Models;

namespace MedLab3.Services
{
    public interface IBannedTokenService
    {
        void CheckAuthentication(TokenBan token);
    }
}
