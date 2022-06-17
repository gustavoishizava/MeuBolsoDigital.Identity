using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace MBD.Identity.Application.Requests;

public abstract class BaseRequest
{
    public abstract ValidationResult Validate();
}