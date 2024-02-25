using MedLab3.Models;
using MedLab3.Models.Doctor;

namespace MedLab3.Services
{
    public interface IDoctorService
    {
        Task<TokenResponseModel> RegisterAsync(DoctorRegisterModel doctorRegisterModel);
        Task<TokenResponseModel> LoginAsync(LoginCredentialsModel LC);
        Task<DoctorProfileModel> GetProfileAsync(Guid userId);
        Task EditProfileAsync(DoctorEditModel doctorEditModel, Guid userId);
        Task LogOutAsync(TokenBan token);
    }
}
