using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.Budgets;
using ERNI.PBA.Server.Business.UnitTests.Commands.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERNI.PBA.Server.Business.UnitTests.Commands.Budgets
{
    [TestClass]
    public class CreateBudgetsForAllActiveUsersCommandTests
    {
        [TestMethod]
        public async Task Ok()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(_ => _.GetAllUsers(It.IsAny<Expression<Func<User, bool>>>(), CancellationToken.None))
            .Returns(Task.FromResult(new User[]
            {
                new User() { Id = 1, FirstName = "Joe"}
            }));

            var budgetRepositoryMock = new Mock<IBudgetRepository>();


            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var subject = new CreateBudgetsForAllActiveUsersCommand(userRepositoryMock.Object, budgetRepositoryMock.Object, unitOfWorkMock.Object);

            await subject.ExecuteAsync(new Domain.Models.Payloads.CreateBudgetsForAllActiveUsersRequest()
            {
                BudgetType = Domain.Enums.BudgetTypeEnum.PersonalBudget
            }, PrincipalBuilder.New().Build(), CancellationToken.None);

            budgetRepositoryMock.Verify(_ => _.AddBudgetAsync(It.IsAny<Budget>()), Times.Once);
            unitOfWorkMock.Verify(_ => _.SaveChanges(CancellationToken.None));
        }
    }
}
