using System.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MBD.Identity.Domain.Interfaces.Services;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace MBD.Identity.Infrastructure.Services
{
    public sealed class JwtService : IJwtService
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public JwtService(SymmetricSecurityKey symmetricSecurityKey)
        {
            _symmetricSecurityKey = symmetricSecurityKey;
        }

        public string Generate(string issuer, string audience, DateTime createdAt, DateTime expiresIn, IEnumerable<Claim> claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = expiresIn,
                NotBefore = createdAt,
                IssuedAt = createdAt,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public string GetEmail(string token)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var result = tokenHandler.ReadJsonWebToken(token);
            return result.Claims.FirstOrDefault(x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
        }

        public bool IsValid(string token, string issuer, string audience)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var result = tokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                RequireSignedTokens = false,
                IssuerSigningKey = _symmetricSecurityKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            });

            return result.IsValid;
        }
    }
}