using System.Linq;
using System;
using System.Collections.Generic;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Identity.API.Models
{
    public class ErrorModel
    {
        public List<string> Errors { get; private set; }

        public ErrorModel(IResult result)
        {
            Errors = result.Message.Split(Environment.NewLine).ToList();
        }
    }
}