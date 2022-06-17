using System;
using System.Threading.Tasks;
using MBD.Identity.Domain.Entities;

namespace MBD.Identity.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid id);
        Task<RefreshToken> GetRefreshTokenByToken(Guid token);
        void Add(User user);
        void AddRefreshToken(RefreshToken refreshToken);
    }
}