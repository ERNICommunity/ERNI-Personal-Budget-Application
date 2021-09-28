using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.InvoiceImages;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERNI.PBA.Server.Business.UnitTests.Commands.InvoiceImages
{
    public class PrincipalBuilder
    {
        private readonly IDictionary<string, string> _claims = new Dictionary<string, string>();

        public static PrincipalBuilder New() => new();

        public PrincipalBuilder WithId(Guid id)
        {
            _claims[UserClaims.Id] = id.ToString();
            return this;
        }

        public PrincipalBuilder WithRandomId()
        {
            _claims[UserClaims.Id] = Guid.NewGuid().ToString();
            return this;
        }

        public ClaimsPrincipal Build()
        {
            var c = new ClaimsPrincipal(new ClaimsIdentity(_claims.Select(_ => new Claim(_.Key, _.Value))));

            return c;
        }
    }


    [TestClass]
    public class AddInvoiceImageCommandTests
    {
        [TestMethod]
        public async Task UnapprovedRequestFails()
        {
            var userGuid = Guid.Parse("72df13d2-25bf-4a1a-b104-65e0ef71cbff");

            var requestRepositoryMock = new Mock<IRequestRepository>();
            requestRepositoryMock.Setup(_ => _.GetRequest(1, CancellationToken.None))
                .Returns((int a, CancellationToken b) => Task.FromResult(new Request()));

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(_ => _.GetUser(userGuid, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new User()));

            var invoiceImageRepositoryMock = new Mock<IInvoiceImageRepository>();
            var unitOwWork = new Mock<IUnitOfWork>();

            var cmd = new AddInvoiceImageCommand(requestRepositoryMock.Object, userRepositoryMock.Object, invoiceImageRepositoryMock.Object, unitOwWork.Object);
            
            await Assert.ThrowsExceptionAsync<OperationErrorException>(() => cmd.ExecuteAsync(
                new AddInvoiceImageCommand.InvoiceImageModel(1, "a.pdf", "application/pdf",
                    Enumerable.Range(1, 20).Select(_ => (byte)_).ToArray()), PrincipalBuilder.New().WithId(userGuid).Build(),
                CancellationToken.None));
        }

        [TestMethod]
        public async Task ApprovedRequestFails()
        {
            var userGuid = Guid.Parse("72df13d2-25bf-4a1a-b104-65e0ef71cbff");

            var requestRepositoryMock = new Mock<IRequestRepository>();
            requestRepositoryMock.Setup(_ => _.GetRequest(1, CancellationToken.None))
                .Returns((int a, CancellationToken b) => Task.FromResult(new Request()
                {
                    State =Domain.Enums.RequestState.Approved
                }));

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(_ => _.GetUser(userGuid, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new User()));

            var invoiceImageRepositoryMock = new Mock<IInvoiceImageRepository>();
            var unitOwWork = new Mock<IUnitOfWork>();

            var cmd = new AddInvoiceImageCommand(requestRepositoryMock.Object, userRepositoryMock.Object, invoiceImageRepositoryMock.Object, unitOwWork.Object);

            await cmd.ExecuteAsync(
                new AddInvoiceImageCommand.InvoiceImageModel(1, "a.pdf", "application/pdf",
                    Enumerable.Range(1, 20).Select(_ => (byte)_).ToArray()), PrincipalBuilder.New().WithId(userGuid).Build(),
                CancellationToken.None);

            invoiceImageRepositoryMock.Verify(_ => _.AddInvoiceImage(It.IsAny<InvoiceImage>(), It.IsAny<CancellationToken>()), Times.Once());
            unitOwWork.Verify(_ => _.SaveChanges(CancellationToken.None), Times.Once());
        }
    }
}
