using System.Threading.Tasks;
using MBD.Identity.Application.Requests;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Identity.Application.Interfaces
{
    public interface IUserAppService
    {
        Task<IResult> CreateAsync(CreateUserRequest request);
    }
}