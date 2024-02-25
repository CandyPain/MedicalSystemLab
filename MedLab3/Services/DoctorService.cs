using MedLab3.Data;
using MedLab3.Models;
using MedLab3.Models.Doctor;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MedLab3.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        private readonly string RegexPhone = @"^\+7\d{10}$";
        private readonly string RegexBirthDate = @"^\d{2}.\d{2}.\d{4} \d{1,2}:\d{2}:\d{2}$";
        private readonly string RegexEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public DoctorService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }

        async Task IDoctorService.EditProfileAsync(DoctorEditModel doctorEditModel, Guid userId)
        {
            var user = await  _context.Doctors.SingleOrDefaultAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new DirectoryNotFoundException();
            }
            if (!Regex.IsMatch(doctorEditModel.Birthday.ToString(), RegexBirthDate) || !Regex.IsMatch(doctorEditModel.Phone, RegexPhone) || !Regex.IsMatch(doctorEditModel.Phone, RegexPhone) || doctorEditModel.Birthday.Value.Year >= DateTime.Now.Year)
            {
                throw new ArgumentException("Bad Data");
            }
            if(user.Email != doctorEditModel.Email && _context.Doctors.SingleOrDefault(user => user.Email == doctorEditModel.Email) != null) {throw new ArgumentException("Email already exist"); }
            user.Birthday = doctorEditModel.Birthday;
            user.Phone = doctorEditModel.Phone;
            user.Gender = doctorEditModel.Gender;
            user.Name = doctorEditModel.Name;
            user.Email = doctorEditModel.Email;
            _context.SaveChanges();
        }

        async Task<DoctorProfileModel> IDoctorService.GetProfileAsync(Guid userId)
        {
            var user = await _context.Doctors.SingleOrDefaultAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new DirectoryNotFoundException();
            }
            DoctorProfileModel userDto = new DoctorProfileModel
            {
                Id = user.Id,
                Email = user.Email,
                Birthday = user.Birthday,
                CreateTime = user.CreateTime,
                Gender = user.Gender,
                Name = user.Name,
                Phone = user.Phone,
            };
            return userDto;
        }

        async Task<TokenResponseModel> IDoctorService.LoginAsync(LoginCredentialsModel LC)
        {
            var user = _context.Doctors.SingleOrDefault(u => u.Email == LC.Email);
            var HashUserPassword = HashPassword(LC.Password);
            if (!Regex.IsMatch(LC.Email, RegexEmail))
            {
                throw new ArgumentException("Bad Email");
            }
            if (user == null || HashUserPassword != user.Password)
            {
                throw new ArgumentException("Bad Data");
            }
            TokenResponseModel token = GenerateToken(user);
            return token;
        }

        async Task IDoctorService.LogOutAsync(TokenBan token)
        {
            _context.TokensBan.Add(token);
            _context.SaveChanges();
        }

        async Task<TokenResponseModel> IDoctorService.RegisterAsync(DoctorRegisterModel doctorRegisterModel)
        {
            if(doctorRegisterModel == null)
            {
                throw new ArgumentException("Bad Data");
            }
            var existingUser = await  _context.Doctors.SingleOrDefaultAsync(user => user.Email == doctorRegisterModel.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("User with this Email has already exist");
            }
            if(_context.Specialitys.SingleOrDefault(s => s.Id ==doctorRegisterModel.Speciality) == null)
            {
                throw new ArgumentException("Bad Speciality Id");
            }
            if (!Regex.IsMatch(doctorRegisterModel.Birthday.ToString(), RegexBirthDate) || !Regex.IsMatch(doctorRegisterModel.Phone, RegexPhone) || !Regex.IsMatch(doctorRegisterModel.Email, RegexEmail) || doctorRegisterModel.Birthday.Value.Year >= DateTime.Now.Year)
            {
                throw new ArgumentException("Bad Data");
            }
            Guid ID = Guid.NewGuid();
            var hashedPassword = HashPassword(doctorRegisterModel.Password);
            var doctor = new DoctorModel
            {
                Id = ID,
                Email = doctorRegisterModel.Email,
                CreateTime = DateTime.Now,
                Password = hashedPassword,
                Name = doctorRegisterModel.Name,
                Birthday = doctorRegisterModel.Birthday,
                Gender = doctorRegisterModel.Gender,
                Phone = doctorRegisterModel.Phone,
                Speciality = doctorRegisterModel.Speciality
            };
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            TokenResponseModel token = GenerateToken(doctor);
            return token;
        }



        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        private TokenResponseModel GenerateToken(DoctorModel user)
        {
            var claims = new List<Claim> {new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Hash, user.Password), 
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())};
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: _configuration["Token:ISSUER"],
                    audience: _configuration["Token:AUDIENCE"],
                    claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:KEY"])), SecurityAlgorithms.HmacSha256));
            return new TokenResponseModel { Token = new JwtSecurityTokenHandler().WriteToken(jwt) };
        }

    }
}
