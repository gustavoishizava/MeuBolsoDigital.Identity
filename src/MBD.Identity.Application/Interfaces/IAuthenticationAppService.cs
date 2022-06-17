using System.Threading.Tasks;
using MBD.Identity.Application.Requests;
using MBD.Identity.Application.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Identity.Application.Interfaces
{
    public interface IAuthenticationAppService
    {
        Task<IResult<AccessTokenResponse>> AuthenticateAsync(AuthenticateRequest request);
        Task<IResult<AccessTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
}