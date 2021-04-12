using System.Collections.Generic;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Domain.Interfaces.Services
{
    public interface IUserResourceService
    {
        Task<IEnumerable<UserResourceModel>> GetAsync();
    }
}