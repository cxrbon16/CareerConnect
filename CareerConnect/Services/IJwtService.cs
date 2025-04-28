using CareerConnect.Models;

namespace CareerConnect.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// Kullanıcı bilgilerine göre bir JWT token üretir.
        /// </summary>
        /// <param name="user">Token üretilecek kullanıcı.</param>
        /// <returns>JWT string olarak döner.</returns>
        string GenerateToken(User user);
    }
}