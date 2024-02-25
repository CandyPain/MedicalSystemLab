using MedLab3.Data;
using MedLab3.Models;
using System.Security.Authentication;

namespace MedLab3.Services
{
    public class BannedTokensService : IBannedTokenService
    {
        private readonly AppDbContext _context;
        public BannedTokensService(AppDbContext context)
        {
            _context = context;
        }

        public void CheckAuthentication(TokenBan token)
        {
            var banToken = _context.TokensBan.FirstOrDefault(t => t.BannedToken == token.BannedToken);
            if (banToken != null)
            {
                throw new AuthenticationException();
            }
        }
    }
}
