using System;
using MBD.Identity.Domain.Constants;
using MBD.Identity.Domain.Interfaces.Services;
using MeuBolsoDigital.Core.Assertions;

namespace MBD.Identity.Domain.ValueObjects
{
    /// <summary>
    /// Deve conter ao menos 1 dígito;
    /// Deve conter ao menos 1 letra minúscula;
    /// Deve conter ao menos 1 letra maiúscula;
    /// Deve conter ao menos 1 caracter especial;
    /// Deve conter ao menos 8 caracteres.
    /// </summary>
    public class StrongPassword
    {
        public string PasswordHash { get; private set; }

        public StrongPassword(string password, IHashService hashService)
        {
            if (hashService == null)
                throw new ArgumentNullException(nameof(hashService), "Hash service cannot be null.");

            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");

            DomainAssertions.ArgumentMatches(RegularExpressions.StrongPassword, password, "Weak password.");

            PasswordHash = hashService.Create(password);
        }

        #region EF
        protected StrongPassword() { }
        #endregion
    }
}