using System;
using System.Threading.Tasks;
using MBD.Identity.Domain.Entities;
using MBD.Identity.Domain.Interfaces.Repositories;
using MBD.Identity.Infrastructure.Context;
using MongoDB.Driver;

namespace MBD.Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityContext _context;

        public UserRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.Collection.Find(x => x.Email.NormalizedAddress == email.ToUpper())
                                                  .FirstOrDefaultAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.Collection.Find(x => x.Id == id)
                                                  .FirstOrDefaultAsync();
        }

        public Task<RefreshToken> GetRefreshTokenByToken(Guid token)
        {
            throw new NotImplementedException();
        }
    }
}