using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Employees
{
    public interface IGetEmployeeCodeQuery : IQuery<EmployeeCodeModel, AdUserOutputModel[]>
    {
    }
}
