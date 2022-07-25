using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using MBD.Identity.API.Controllers;
using MBD.Identity.API.Models;
using MBD.Identity.Application.Interfaces;
using MBD.Identity.Application.Requests;
using MBD.Identity.Application.Responses;
using MeuBolsoDigital.Application.Utils.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Identity.UnitTests.API.Controllers
{
    public class AuthenticationControllerTests
    {
        private readonly AutoMocker _mocker;
        private readonly Faker _faker;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _mocker = new AutoMocker();
            _faker = new Faker();
            _controller = _mocker.CreateInstance<AuthenticationController>();
        }

        [Fact]
        public async Task Authenticate_Error_ReturnBadRequest()
        {
            // Arrange
            var request = new AuthenticateRequest
            {
                Email = _faker.Person.Email,
                Password = Guid.NewGuid().ToString()
            };

            var result = Result<AccessTokenResponse>.Fail(Guid.NewGuid().ToString());

            _mocker.GetMock<IAuthenticationAppService>()
                .Setup(x => x.AuthenticateAsync(request))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.Authenticate(request) as ObjectResult;

            // Assert
            var value = response.Value as ErrorModel;
            Assert.IsType<ErrorModel>(response.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(result.Message, value.Errors.First());
            _mocker.GetMock<IAuthenticationAppService>().Verify(x => x.AuthenticateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Authenticate_Success_ReturnOk()
        {
            // Arrange
            var request = new AuthenticateRequest
            {
                Email = _faker.Person.Email,
                Password = Guid.NewGuid().ToString()
            };

            var accessTokenResult = new AccessTokenResponse
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                ExpiresIn = new Random().Next(100, 1000)
            };

            var result = Result<AccessTokenResponse>.Success(accessTokenResult);

            _mocker.GetMock<IAuthenticationAppService>()
                .Setup(x => x.AuthenticateAsync(request))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.Authenticate(request) as ObjectResult;

            // Assert
            var value = response.Value as AccessTokenResponse;
            Assert.IsType<AccessTokenResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(accessTokenResult.AccessToken, value.AccessToken);
            Assert.Equal(accessTokenResult.RefreshToken, value.RefreshToken);
            Assert.Equal(accessTokenResult.CreatedAt, value.CreatedAt);
            Assert.Equal(accessTokenResult.ExpiresIn, value.ExpiresIn);
            _mocker.GetMock<IAuthenticationAppService>().Verify(x => x.AuthenticateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Refresh_Error_ReturnBadRequest()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = Guid.NewGuid().ToString()
            };

            var result = Result<AccessTokenResponse>.Fail(Guid.NewGuid().ToString());

            _mocker.GetMock<IAuthenticationAppService>()
                .Setup(x => x.RefreshTokenAsync(request))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.Refresh(request) as ObjectResult;

            // Assert
            var value = response.Value as ErrorModel;
            Assert.IsType<ErrorModel>(response.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(result.Message, value.Errors.First());
            _mocker.GetMock<IAuthenticationAppService>().Verify(x => x.RefreshTokenAsync(request), Times.Once);
        }

        [Fact]
        public async Task Refresh_Success_ReturnOk()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = Guid.NewGuid().ToString()
            };

            var accessTokenResult = new AccessTokenResponse
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                ExpiresIn = new Random().Next(100, 1000)
            };

            var result = Result<AccessTokenResponse>.Success(accessTokenResult);

            _mocker.GetMock<IAuthenticationAppService>()
                .Setup(x => x.RefreshTokenAsync(request))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.Refresh(request) as ObjectResult;

            // Assert
            var value = response.Value as AccessTokenResponse;
            Assert.IsType<AccessTokenResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(accessTokenResult.AccessToken, value.AccessToken);
            Assert.Equal(accessTokenResult.RefreshToken, value.RefreshToken);
            Assert.Equal(accessTokenResult.CreatedAt, value.CreatedAt);
            Assert.Equal(accessTokenResult.ExpiresIn, value.ExpiresIn);
            _mocker.GetMock<IAuthenticationAppService>().Verify(x => x.RefreshTokenAsync(request), Times.Once);
        }
    }
}