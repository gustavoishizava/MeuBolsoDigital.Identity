using System;
using System.Threading.Tasks;
using Bogus;
using MBD.Identity.Application.Interfaces;
using MBD.Identity.Application.Requests;
using MBD.Identity.Application.Responses;
using MBD.Identity.Application.Services;
using MBD.Identity.Domain.Interfaces.Services;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;
using static MBD.Identity.Domain.ValueObjects.AccessTokenResponse;

namespace MBD.Identity.UnitTests.Application.Services
{
    public class AuthenticationAppServiceTests
    {
        private readonly IAuthenticationAppService _authAppService;
        private readonly AutoMocker _mocker;
        private readonly Faker _faker;

        public AuthenticationAppServiceTests()
        {
            _mocker = new AutoMocker();
            _authAppService = _mocker.CreateInstance<AuthenticationAppService>();
            _faker = new Faker();
        }

        [Theory]
        [InlineData("invalid-email", "")]
        [InlineData("", null)]
        [InlineData(null, null)]
        public async Task InvalidRequest_Authenticate_ReturnFail(string email, string password)
        {
            // Arrange
            var request = new AuthenticateRequest
            {
                Email = email,
                Password = password
            };

            // Act
            var result = await _authAppService.AuthenticateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);
            _mocker.GetMock<IAuthenticationService>().Verify(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ValidRequest_Authenticate_ErrorOnAuthenticate_ReturnFail()
        {
            // Arrange
            var request = new AuthenticateRequest
            {
                Email = _faker.Person.Email,
                Password = _faker.Random.AlphaNumeric(8)
            };

            var errorMessage = _faker.Lorem.Text();

            _mocker.GetMock<IAuthenticationService>()
                   .Setup(x => x.AuthenticateAsync(request.Email, request.Password))
                   .ReturnsAsync(AccessTokenResponseFactory.Fail(errorMessage));

            // Act
            var result = await _authAppService.AuthenticateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.Equal(errorMessage, result.Message);
            _mocker.GetMock<IAuthenticationService>().Verify(x => x.AuthenticateAsync(request.Email, request.Password), Times.Once);
        }

        [Fact]
        public async Task ValidRequest_Authenticate_ReturnSuccess()
        {
            // Arrange
            var request = new AuthenticateRequest
            {
                Email = _faker.Person.Email,
                Password = _faker.Random.AlphaNumeric(8)
            };

            var accessToken = Guid.NewGuid().ToString();
            var refreshToken = Guid.NewGuid().ToString();
            var expiresInSeconds = _faker.Random.Int(3600, 10000);
            var createdAt = DateTime.Now;

            _mocker.GetMock<IAuthenticationService>()
                   .Setup(x => x.AuthenticateAsync(request.Email, request.Password))
                   .ReturnsAsync(AccessTokenResponseFactory.Success(accessToken, refreshToken, expiresInSeconds, createdAt));

            // Act
            IResult<AccessTokenResponse> result = await _authAppService.AuthenticateAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);
            Assert.Equal(accessToken, result.Data.AccessToken);
            Assert.Equal(refreshToken, result.Data.RefreshToken);
            Assert.Equal(expiresInSeconds, result.Data.ExpiresIn);
            Assert.Equal(createdAt, result.Data.CreatedAt);
            _mocker.GetMock<IAuthenticationService>().Verify(x => x.AuthenticateAsync(request.Email, request.Password), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task InvalidRequest_RefreshToken_ReturnFail(string token)
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = token
            };

            // Act
            var result = await _authAppService.RefreshTokenAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);
            _mocker.GetMock<IAuthenticationService>().Verify(x => x.AuthenticateByRefreshTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ValidRequest_RefreshToken_ErrorOnAuthenticate_ReturnFail()
        {
            // Arrange
            var token = Guid.NewGuid().ToString();
            var request = new RefreshTokenRequest
            {
                RefreshToken = token
            };

            var errorMessage = _faker.Lorem.Text();

            _mocker.GetMock<IAuthenticationService>()
                   .Setup(x => x.AuthenticateByRefreshTokenAsync(request.RefreshToken))
                   .ReturnsAsync(AccessTokenResponseFactory.Fail(errorMessage));

            // Act
            var result = await _authAppService.RefreshTokenAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.Equal(errorMessage, result.Message);
            _mocker.GetMock<IAuthenticationService>().Verify(x => x.AuthenticateByRefreshTokenAsync(request.RefreshToken), Times.Once);
        }

        [Fact]
        public async Task ValidRequest_RefreshToken_ReturnSuccess()
        {
            // Arrange
            var token = Guid.NewGuid().ToString();
            var request = new RefreshTokenRequest
            {
                RefreshToken = token
            };

            var accessToken = Guid.NewGuid().ToString();
            var refreshToken = Guid.NewGuid().ToString();
            var expiresInSeconds = _faker.Random.Int(3600, 10000);
            var createdAt = DateTime.Now;

            _mocker.GetMock<IAuthenticationService>()
                   .Setup(x => x.AuthenticateByRefreshTokenAsync(request.RefreshToken))
                   .ReturnsAsync(AccessTokenResponseFactory.Success(accessToken, refreshToken, expiresInSeconds, createdAt));

            // Act
            IResult<AccessTokenResponse> result = await _authAppService.RefreshTokenAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);
            Assert.Equal(accessToken, result.Data.AccessToken);
            Assert.Equal(refreshToken, result.Data.RefreshToken);
            Assert.Equal(expiresInSeconds, result.Data.ExpiresIn);
            Assert.Equal(createdAt, result.Data.CreatedAt);
            _mocker.GetMock<IAuthenticationService>().Verify(x => x.AuthenticateByRefreshTokenAsync(request.RefreshToken), Times.Once);
        }
    }
}