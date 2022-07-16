using System;
using System.Text;
using System.Threading.Tasks;
using MBD.Identity.Domain.Configuration;
using MBD.Identity.Domain.Entities;
using MBD.Identity.Domain.Interfaces.Repositories;
using MBD.Identity.Domain.Interfaces.Services;
using MBD.Identity.Domain.Services;
using MBD.Identity.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Identity.UnitTests.Domain.Services
{
    public class AuthenticationServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly AuthenticationService _authenticationService;
        private readonly User _existingUser;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly string _validEmail;
        private readonly string _validPassword;

        public AuthenticationServiceTests()
        {
            var secret = Guid.NewGuid();
            _mocker = new AutoMocker();
            _mocker.Use<IHashService>(new HashService());
            _mocker.Use<IJwtService>(new JwtService(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.ToString()))));
            _mocker.Use<IOptions<JwtConfiguration>>(
                Options.Create(
                    _jwtConfiguration = new JwtConfiguration { Secret = secret.ToString(), Audience = "TEST", Issuer = "TEST", ExpiresInSeconds = 10, RefreshExpiresInSeconds = 15 }));
            _authenticationService = _mocker.CreateInstance<AuthenticationService>(); ;

            _validEmail = "gustavo@gmail.com";
            _validPassword = "P@ssw0rd!";
            _existingUser = new User("Valid user", _validEmail, _validPassword, new HashService());
        }

        [Fact(DisplayName = "Autenticar com e-mail e senha válido.")]
        public async Task ValidEmail_Authenticate_ReturnSuccess()
        {
            // Arrange
            var currentDate = DateTime.Now;
            _mocker.GetMock<IUserRepository>().Setup(method => method.GetByEmailAsync(_validEmail)).ReturnsAsync(_existingUser);

            // Act
            var accessTokenResponse = await _authenticationService.AuthenticateAsync(_validEmail, _validPassword);

            // Assert
            _mocker.GetMock<IUserRepository>().Verify(repository => repository.GetByEmailAsync(_validEmail), Times.Once);

            Assert.False(accessTokenResponse.HasErrors);
            Assert.NotEmpty(accessTokenResponse.AccessToken);
            Assert.NotEmpty(accessTokenResponse.RefreshToken);
            Assert.True(accessTokenResponse.CreatedAt >= currentDate);
            Assert.Equal(_jwtConfiguration.ExpiresInSeconds, accessTokenResponse.ExpiresIn);
        }

        [Theory(DisplayName = "Autenticar com e-mail e/ou senha inválido(s) deve retornar erro.")]
        [InlineData("email@email.com", "P@ssw0rd!")]
        [InlineData("gustavo@gmail.com", "invalidpassword")]
        [InlineData("email@email.com", "invalidpassword")]
        [InlineData("", "")]
        public async Task InvalidEmailOrPassword_Authenticate_RetrunError(string email, string password)
        {
            // Arrange
            _mocker.GetMock<IUserRepository>().Setup(method => method.GetByEmailAsync(_validEmail)).ReturnsAsync(_existingUser);

            // Act
            var accessTokenResponse = await _authenticationService.AuthenticateAsync(email, password);

            // Assert
            _mocker.GetMock<IUserRepository>().Verify(repository => repository.GetByEmailAsync(email), string.IsNullOrEmpty(email) ? Times.Never : Times.Once);

            Assert.True(accessTokenResponse.HasErrors);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task RefreshTokenIsNullOrEmpty_ReturnFail(string refreshtoken)
        {
            // Act
            var result = await _authenticationService.AuthenticateByRefreshTokenAsync(refreshtoken);

            // Assert
            Assert.True(result.HasErrors);
            Assert.Equal("Este token não está mais válido.", result.Error);
            _mocker.GetMock<IUserRepository>().Verify(repository => repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task InvalidRefreshToken_ReturnFail()
        {
            // Arrange
            var refreshToken = Guid.NewGuid().ToString();

            // Act
            var result = await _authenticationService.AuthenticateByRefreshTokenAsync(refreshToken);

            // Assert
            Assert.True(result.HasErrors);
            Assert.Equal("Este token não está mais válido.", result.Error);
            _mocker.GetMock<IUserRepository>().Verify(repository => repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task InvalidEmail_ReturnFail()
        {
            // Arrange
            _mocker.GetMock<IUserRepository>().Setup(method => method.GetByEmailAsync(_validEmail)).ReturnsAsync(_existingUser);
            var authResponse = await _authenticationService.AuthenticateAsync(_validEmail, _validPassword);

            _mocker.GetMock<IUserRepository>()
                .Setup(x => x.GetByEmailAsync(_validEmail.ToUpper()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authenticationService.AuthenticateByRefreshTokenAsync(authResponse.RefreshToken);

            // Assert
            Assert.True(result.HasErrors);
            Assert.Equal("Este token não está mais válido.", result.Error);
            _mocker.GetMock<IUserRepository>().Verify(repository => repository.GetByEmailAsync(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ValidRefreshToken_ReturnSuccess()
        {
            // Arrange
            var currentDate = DateTime.Now;
            _mocker.GetMock<IUserRepository>().Setup(method => method.GetByEmailAsync(_validEmail)).ReturnsAsync(_existingUser);
            var authResponse = await _authenticationService.AuthenticateAsync(_validEmail, _validPassword);
            _mocker.GetMock<IUserRepository>().Setup(method => method.GetByEmailAsync(_validEmail.ToUpper())).ReturnsAsync(_existingUser);

            // Act
            var result = await _authenticationService.AuthenticateByRefreshTokenAsync(authResponse.RefreshToken);

            // Assert
            Assert.False(result.HasErrors);
            Assert.NotEmpty(result.AccessToken);
            Assert.NotEmpty(result.RefreshToken);
            Assert.True(result.CreatedAt >= currentDate);
            Assert.Equal(_jwtConfiguration.ExpiresInSeconds, result.ExpiresIn);

            _mocker.GetMock<IUserRepository>().Verify(repository => repository.GetByEmailAsync(_validEmail.ToUpper()), Times.AtLeastOnce);
        }
    }
}