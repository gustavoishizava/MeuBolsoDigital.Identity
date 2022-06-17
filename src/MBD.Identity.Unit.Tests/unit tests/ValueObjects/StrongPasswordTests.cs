using System;
using MBD.Core.DomainObjects;
using MBD.Identity.Domain.Interfaces.Services;
using MBD.Identity.Domain.ValueObjects;
using MBD.Identity.Infrastructure.Services;
using Xunit;

namespace MBD.Identity.Unit.Tests.unit_tests.ValueObjects
{
    public class StrongPasswordTests
    {
        private readonly IHashService _hashService;

        public StrongPasswordTests()
        {
            _hashService = new HashService();
        }

        [Theory(DisplayName = "Senha fraca que não corresponde ao padrão exigido pelo domínio.")]
        [InlineData("1234")]
        [InlineData("12345678")]
        [InlineData("abc123")]
        [InlineData("abc123@")]
        [InlineData("AbC123@")]
        [InlineData("$*&@#")]
        [InlineData("$a*c&122@#")]
        public void WeakPassword_NewPassword_ReturnDomainException(string weakPassword)
        {
            // Act && Assert
            Assert.Throws<DomainException>(() => new StrongPassword(weakPassword, _hashService));
        }

        [Theory(DisplayName = "Senha forte correspondente ao padrão exigido pelo domínio.")]
        [InlineData("Abcd@12345")]
        [InlineData("12345#@aAf")]
        [InlineData("1234t56@A")]
        [InlineData("@bcdeF123")]
        public void StrongPassword_NewPassword_ReturnSuccess(string strongPassword)
        {
            // Arrange && Act
            var password = new StrongPassword(strongPassword, _hashService);

            // Assert
            Assert.True(_hashService.IsMatch(strongPassword, password.PasswordHash));
        }

        [Fact(DisplayName = "Parâmetros inválidos devem retornar exceção.")]
        public void InvalidArguments_NewPassword_ReturnArgumentNullException()
        {
            // Arrange && Act && Assert
            Assert.Throws<ArgumentNullException>(() => new StrongPassword(string.Empty, _hashService));
            Assert.Throws<ArgumentNullException>(() => new StrongPassword("password", null));
        }
    }
}