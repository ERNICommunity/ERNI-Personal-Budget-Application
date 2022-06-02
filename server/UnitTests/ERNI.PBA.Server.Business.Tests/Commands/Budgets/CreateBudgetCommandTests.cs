using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.API;
using ERNI.PBA.Server.Business.Commands.Budgets;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERNI.PBA.Server.Business.UnitTests.Commands.Budgets
{
    [TestClass]
    public class CreateBudgetCommandTests
    {
        [TestMethod]
        public async Task ValidRequestCreatesBudget()
        {
            var token = CancellationToken.None;
            var userRepository = new Mock<IUserRepository>();

            userRepository.Setup(_ => _.GetUser(1, token)).Returns(Task.FromResult(new User
            {
                Id = 1,
                State = UserState.Active
            }));

            var budgetRepository = new Mock<IBudgetRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var command = new CreateBudgetCommand(userRepository.Object, budgetRepository.Object, unitOfWork.Object);

            await command.ExecuteAsync(new CreateBudgetRequest
            {
                Amount = 100,
                BudgetType = BudgetTypeEnum.RecreationBudget,
                Title = "Budget",
                UserId = 1
            }, new ClaimsPrincipal(), token);

            budgetRepository.Verify(_ => _.AddBudgetAsync(It.Is<Budget>(b =>
                b.UserId == 1 && b.Amount == 100 && b.BudgetType == BudgetTypeEnum.RecreationBudget &&
                b.Title == "Budget")));

            unitOfWork.Verify(_ => _.SaveChanges(token));
        }
    }
}