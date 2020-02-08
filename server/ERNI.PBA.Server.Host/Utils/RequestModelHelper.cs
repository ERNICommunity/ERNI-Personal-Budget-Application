using System.Linq;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;

namespace ERNI.PBA.Server.Host.Utils
{
    public static class RequestModelHelper
    {
        public static RequestModel GetModel(TeamRequest teamRequest)
        {
            return new RequestModel
            {
                Id = teamRequest.Id,
                Title = teamRequest.Title,
                Amount = teamRequest.Requests.Sum(_ => _.Amount),
                Year = teamRequest.Year,
                Date = teamRequest.Date,
                State = teamRequest.State,
                User = new UserModel
                {
                    Id = teamRequest.UserId,
                    FirstName = teamRequest.User.FirstName,
                    LastName = teamRequest.User.LastName
                },
                Budget = new BudgetModel
                {
                    Title = BudgetType.Types.Single(_ => _.Id == BudgetTypeEnum.TeamBudget).Name,
                    Type = BudgetTypeEnum.TeamBudget
                }
            };
        }

        public static RequestModel GetModel(Request request)
        {
            return new RequestModel
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Amount,
                Year = request.Year,
                Date = request.Date,
                State = request.State,
                User = new UserModel
                {
                    Id = request.UserId,
                    FirstName = request.Budget.User.FirstName,
                    LastName = request.Budget.User.LastName
                },
                Budget = new BudgetModel
                {
                    Id = request.BudgetId,
                    Title = request.Budget.Title,
                    Type = request.Budget.BudgetType
                }
            };
        }
    }
}
