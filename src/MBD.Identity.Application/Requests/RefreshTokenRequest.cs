using FluentValidation;
using FluentValidation.Results;

namespace MBD.Identity.Application.Requests;

public class RefreshTokenRequest : BaseRequest
{
    public string RefreshToken { get; set; }

    public override ValidationResult Validate()
    {
        return new RefreshTokenValidation().Validate(this);
    }

    public class RefreshTokenValidation : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenValidation()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }
}
