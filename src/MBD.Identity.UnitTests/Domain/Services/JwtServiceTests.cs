using System.Collections.Generic;
using System;
using System.Text;
using MBD.Identity.Domain.Interfaces.Services;
using MBD.Identity.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MBD.Identity.UnitTests.Domain.Services
{
    public class JwtServiceTests
    {
        private readonly IJwtService _jwtService;

        public JwtServiceTests()
        {
            _jwtService = new JwtService(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())));
        }

        [Fact(DisplayName = "Gerar novo JWT.")]
        public void ValidData_NewToken_ReturnSuccess()
        {
            // Arrange
            var issuer = "Service";
            var audience = "Test";
            var dateNow = DateTime.Now;
            var expiresAt = dateNow.AddHours(1);

            // Act
            var jwtGenerated = _jwtService.Generate(issuer, audience, DateTime.Now, expiresAt, new List<Claim>());

            // Assert
            Assert.NotEmpty(jwtGenerated);
            Assert.NotNull(jwtGenerated);
        }

        [Fact]
        public void Validate_ReturnTrue()
        {
            // Arrange
            var issuer = "Service";
            var audience = "Test";
            var dateNow = DateTime.Now;
            var expiresAt = dateNow.AddHours(1);

            var token = _jwtService.Generate(issuer, audience, DateTime.Now, expiresAt, new List<Claim>());

            // Act
            var isValid = _jwtService.IsValid(token, issuer, audience);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Validate_ReturnFalse()
        {
            // Arrange
            var issuer = "Service";
            var audience = "Test";
            var dateNow = DateTime.Now;
            var expiresAt = dateNow.AddHours(1);

            var token = _jwtService.Generate(issuer, audience, DateTime.Now, expiresAt, new List<Claim>());

            // Act
            var isValid = _jwtService.IsValid(token, "Service2", "Test2");

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task ExpiredToken_Validate_ReturnFalse()
        {
            // Arrange
            var issuer = "Service";
            var audience = "Test";
            var dateNow = DateTime.Now;
            var expiresAt = dateNow.AddSeconds(1);

            var token = _jwtService.Generate(issuer, audience, DateTime.Now, expiresAt, new List<Claim>());

            await Task.Delay(1000);

            // Act
            var isValid = _jwtService.IsValid(token, issuer, audience);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void GetEmail_ReturnEmail()
        {
            // Arrange
            var issuer = "Service";
            var audience = "Test";
            var dateNow = DateTime.Now;
            var expiresAt = dateNow.AddHours(1);
            var expectedEmail = "gustavo@gmail.com";

            var token = _jwtService.Generate(issuer, audience, DateTime.Now, expiresAt, new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, expectedEmail)
            });

            // Act
            var email = _jwtService.GetEmail(token);

            // Assert
            Assert.Equal(expectedEmail, email);
        }

        [Fact]
        public void GetEmail_ClaimNotExists_ReturnNull()
        {
            // Arrange
            var issuer = "Service";
            var audience = "Test";
            var dateNow = DateTime.Now;
            var expiresAt = dateNow.AddHours(1);

            var token = _jwtService.Generate(issuer, audience, DateTime.Now, expiresAt, new List<Claim>());

            // Act
            var email = _jwtService.GetEmail(token);

            // Assert
            Assert.Null(email);
        }
    }
}