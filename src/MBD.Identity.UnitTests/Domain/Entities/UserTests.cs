using System;
using MBD.Identity.Domain.Entities;
using MBD.Identity.Domain.Interfaces.Services;
using MBD.Identity.Infrastructure.Services;
using MeuBolsoDigital.Core.Exceptions;
using Xunit;

namespace MBD.Identity.UnitTests.Domain.Entities
{
    public class UserTests
    {
        private readonly IHashService _hashService;

        private readonly User _validUser;

        public UserTests()
        {
            _hashService = new HashService();

            _validUser = new User("Valid user", "user@user.com", "P@ssw0rd!", _hashService);
        }

        [Fact(DisplayName = "Criar usuário inválido.")]
        public void InvalidUser_NewUser_ReturnArgumentExceptionDomainException()
        {
            // Arrange && Act && Assert
            Assert.Throws<ArgumentException>(() =>
                new User("User", "invalid_email", "Abc@122357#", _hashService));

            Assert.Throws<DomainException>(() =>
                new User("User", "email@email.com", "password", _hashService));

            Assert.Throws<DomainException>(() =>
                new User("", "email@email.com", "password", _hashService));
        }

        [Theory(DisplayName = "Criar usuário válido.")]
        [InlineData("Gustavo", "gustavo@gmail.com", "P@ssw0rd!")]
        [InlineData("Eduarda", "eduarda@hotmail.com", "S3nh@4#5a")]
        [InlineData("Vitória", "vitoria_email@hotmail.com.br", "Senh@4#5a")]
        public void ValidEmail_NewUser_ReturnSuccess(string name, string email, string password)
        {
            // Arrange
            var normalizedEmail = email.ToUpper();

            // Act
            var user = new User(name, email, password, _hashService);

            // Assert
            Assert.Equal(name, user.Name);
            Assert.Equal(email, user.Email.Address);
            Assert.Equal(normalizedEmail, user.Email.NormalizedAddress);
            Assert.True(_hashService.IsMatch(password, user.Password.PasswordHash));
        }
    }
}