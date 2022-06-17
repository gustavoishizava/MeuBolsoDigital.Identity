using FluentValidation.Results;

namespace MBD.Identity.Application.Requests;

public abstract class BaseRequest
{
    public abstract ValidationResult Validate();
}