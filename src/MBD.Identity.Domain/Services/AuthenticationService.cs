using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using MBD.Identity.Domain.Configuration;
using MBD.Identity.Domain.Entities;
using MBD.Identity.Domain.Interfaces.Repositories;
using MBD.Identity.Domain.Interfaces.Services;
using MBD.Identity.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using static MBD.Identity.Domain.ValueObjects.AccessTokenResponse;

namespace MBD.Identity.Domain.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IHashService _hashService;
        private readonly JwtConfiguration _jwtConfiguration;

        public AuthenticationService(IUserRepository userRepository, IJwtService jwtService, IHashService hashService, IOptions<JwtConfiguration> jwtConfiguration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
            _jwtConfiguration = jwtConfiguration?.Value ?? throw new ArgumentNullException(nameof(jwtConfiguration));
        }

        public async Task<AccessTokenResponse> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return AccessTokenResponseFactory.Fail("Informe o e-email e/ou senha.");

            var user = await _userRepository.GetByEmailAsync(email.ToLower());
            if (user == null)
                return AccessTokenResponseFactory.Fail("E-mail e/ou senha incorreto(s).");

            if (!_hashService.IsMatch(password, user.Password.PasswordHash))
                return AccessTokenResponseFactory.Fail("E-mail e/ou senha incorreto(s).");

            var issuedAt = DateTime.Now;
            var accessToken = GenerateEncodedJwt(user, issuedAt, _jwtConfiguration.ExpiresInSeconds);
            var refreshToken = GenerateEncodedJwt(user, issuedAt, _jwtConfiguration.RefreshExpiresInSeconds);

            return AccessTokenResponseFactory.Success(accessToken, refreshToken, _jwtConfiguration.ExpiresInSeconds, issuedAt);
        }

        public async Task<AccessTokenResponse> AuthenticateByRefreshTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return AccessTokenResponseFactory.Fail("Este token não está mais válido.");

            var isValid = _jwtService.IsValid(token, _jwtConfiguration.Issuer, _jwtConfiguration.Audience);
            if (!isValid)
                return AccessTokenResponseFactory.Fail("Este token não está mais válido.");

            var user = await _userRepository.GetByEmailAsync(_jwtService.GetEmail(token));
            if (user == null)
                return AccessTokenResponseFactory.Fail("Este token não está mais válido.");

            var issuedAt = DateTime.Now;
            var accessToken = GenerateEncodedJwt(user, issuedAt, _jwtConfiguration.ExpiresInSeconds);
            var refreshToken = GenerateEncodedJwt(user, issuedAt, _jwtConfiguration.RefreshExpiresInSeconds);

            return AccessTokenResponseFactory.Success(accessToken, refreshToken, _jwtConfiguration.ExpiresInSeconds, issuedAt);
        }

        private string GenerateEncodedJwt(User user, DateTime issuedAt, int expiresInSeconds)
        {
            var expiresIn = issuedAt.AddSeconds(_jwtConfiguration.RefreshExpiresInSeconds);
            var claims = GenerateClaims(user.Id, user.Email.NormalizedAddress, issuedAt);
            var encodedJwt = _jwtService.Generate(_jwtConfiguration.Issuer, _jwtConfiguration.Audience, issuedAt, expiresIn, claims);

            return encodedJwt;
        }

        private static IEnumerable<Claim> GenerateClaims(Guid userId, string email, DateTime issuedAt)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(issuedAt).ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(issuedAt).ToString())
            };
            return claims;
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);
    }
}