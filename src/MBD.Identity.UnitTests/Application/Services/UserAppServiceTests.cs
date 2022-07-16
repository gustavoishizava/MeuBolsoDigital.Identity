using System.Threading.Tasks;
using Bogus;
using MBD.Identity.Application.Interfaces;
using MBD.Identity.Application.Requests;
using MBD.Identity.Application.Services;
using MBD.Identity.Domain.Interfaces.Services;
using Moq;
using Moq.AutoMock;
using Xunit;
using static MBD.Identity.Domain.ValueObjects.CreateUserResult;

namespace MBD.Identity.UnitTests.Application.Services
{
    public class UserAppServiceTests
    {
        private readonly IUserAppService _userAppService;
        private readonly AutoMocker _mocker;
        private readonly Faker _faker;

        public UserAppServiceTests()
        {
            _mocker = new AutoMocker();
            _faker = new Faker();
            _userAppService = _mocker.CreateInstance<UserAppService>();
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData("", "", "", "")]
        public async Task InvalidRequest_Create_ReturnFail(string name, string email, string password, string repeatPassword)
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Name = name,
                Email = email,
                Password = password,
                RepeatPassword = repeatPassword
            };

            // Act
            var result = await _userAppService.CreateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);
        }

        [Theory]
        [InlineData("invalid-email", "weak-password")]
        public async Task InvalidEmailAndPassword_Create_ReturnFail(string email, string password)
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Name = _faker.Person.FullName,
                Email = email,
                Password = password,
                RepeatPassword = password
            };

            // Act
            var result = await _userAppService.CreateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);
        }

        [Fact]
        public async Task ValidRequest_Create_ErrorOnCreate_ReturnFail()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Name = _faker.Person.FullName,
                Email = _faker.Person.Email,
                Password = "Test@123",
                RepeatPassword = "Test@123"
            };

            var errorMessage = _faker.Lorem.Text();

            _mocker.GetMock<ICreateUserService>()
                .Setup(x => x.CreateAsync(request.Name, request.Email, request.Password))
                .ReturnsAsync(CreateUserResultFactory.Fail(errorMessage));

            // Act
            var result = await _userAppService.CreateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);
        }

        [Fact]
        public async Task ValidRequest_Create_ReturnSuccess()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Name = _faker.Person.FullName,
                Email = _faker.Person.Email,
                Password = "Test@123",
                RepeatPassword = "Test@123"
            };

            var successMessage = _faker.Lorem.Text();

            _mocker.GetMock<ICreateUserService>()
                .Setup(x => x.CreateAsync(request.Name, request.Email, request.Password))
                .ReturnsAsync(CreateUserResultFactory.Success(successMessage));

            // Act
            var result = await _userAppService.CreateAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotEmpty(result.Message);
        }
    }
}