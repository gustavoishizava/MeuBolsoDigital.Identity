using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using MBD.Identity.API.Controllers;
using MBD.Identity.API.Models;
using MBD.Identity.Application.Interfaces;
using MBD.Identity.Application.Requests;
using MeuBolsoDigital.Application.Utils.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Identity.UnitTests.API.Controllers
{
    public class UsersControllerTests
    {
        private readonly AutoMocker _mocker;
        private readonly Faker _faker;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mocker = new AutoMocker();
            _faker = new Faker();
            _controller = _mocker.CreateInstance<UsersController>();
        }

        [Fact]
        public async Task Create_Error_ReturnBadRequest()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Email = _faker.Person.Email,
                Name = _faker.Person.FullName,
                Password = Guid.NewGuid().ToString(),
                RepeatPassword = Guid.NewGuid().ToString()
            };

            var result = Result.Fail(Guid.NewGuid().ToString());

            _mocker.GetMock<IUserAppService>()
                .Setup(x => x.CreateAsync(request))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.Create(request) as ObjectResult;

            // Assert
            var value = response.Value as ErrorModel;
            Assert.IsType<ErrorModel>(response.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(result.Message, value.Errors.First());
            _mocker.GetMock<IUserAppService>().Verify(x => x.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Create_Success_ReturnOk()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Email = _faker.Person.Email,
                Name = _faker.Person.FullName,
                Password = Guid.NewGuid().ToString(),
                RepeatPassword = Guid.NewGuid().ToString()
            };

            var result = Result.Success();

            _mocker.GetMock<IUserAppService>()
                .Setup(x => x.CreateAsync(request))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.Create(request) as NoContentResult;

            // Assert
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            _mocker.GetMock<IUserAppService>().Verify(x => x.CreateAsync(request), Times.Once);
        }
    }
}