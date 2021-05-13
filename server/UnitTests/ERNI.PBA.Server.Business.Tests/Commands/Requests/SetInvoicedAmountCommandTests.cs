using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.Requests;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERNI.PBA.Server.Business.UnitTests.Commands.Requests
{
    [TestClass]
    public class SetInvoicedAmountCommandTests
    {
        [TestMethod]
        public async Task ValidRequestUpdatesRequest()
        {
            var userId = "F8CF50DB-16C3-4545-B868-A7410020B01D";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new [] {new Claim(UserClaims.Id, userId) }));

            var token = CancellationToken.None;
            var userRepository = new Mock<IUserRepository>();

            userRepository.Setup(_ => _.GetUser(Guid.Parse(userId), token)).Returns(Task.FromResult(new User
            {
                Id = 56,
                State = UserState.Active
            }));

            var request = new Request()
            {
                Id = 42,
                State = RequestState.Approved,
                Amount = 100,
                User = new User {Id = 56},
                Transactions = new[] {new Transaction()}
            };

            var requestRepository = new Mock<IRequestRepository>();

            requestRepository.Setup(_ => _.GetRequest(42, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(request));

            var unitOfWork = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<SetInvoicedAmountCommand>>();

            var command = new SetInvoicedAmountCommand(userRepository.Object, requestRepository.Object, unitOfWork.Object, loggerMock.Object);

            await command.ExecuteAsync((42, new SetInvoicedAmountModel()
            {
                Amount = 99
            }), user, token);

            Assert.AreEqual(99, request.InvoicedAmount);
            Assert.AreEqual(99, request.Transactions.Single().Amount);

            unitOfWork.Verify(_ => _.SaveChanges(token));
        }

        [TestMethod]
        public async Task UserIsNotTheOwner_Invalid()
        {
            var userId = "F8CF50DB-16C3-4545-B868-A7410020B01D";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(UserClaims.Id, userId) }));

            var token = CancellationToken.None;
            var userRepository = new Mock<IUserRepository>();

            userRepository.Setup(_ => _.GetUser(Guid.Parse(userId), token)).Returns(Task.FromResult(new User
            {
                Id = 57,
                State = UserState.Active
            }));

            var request = new Request()
            {
                Id = 42,
                State = RequestState.Approved,
                Amount = 10,
                User = new User { Id = 56 },
                Transactions = new[] { new Transaction() }
            };

            var requestRepository = new Mock<IRequestRepository>();

            requestRepository.Setup(_ => _.GetRequest(42, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(request));

            var unitOfWork = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<SetInvoicedAmountCommand>>();

            var command = new SetInvoicedAmountCommand(userRepository.Object, requestRepository.Object, unitOfWork.Object, loggerMock.Object);

            var exception = await Assert.ThrowsExceptionAsync<OperationErrorException>(() =>
                command.ExecuteAsync((42, new SetInvoicedAmountModel() { Amount = 99 }), user, token));

            Assert.AreEqual(ErrorCodes.AccessDenied, exception.Code);
        }

        [TestMethod]
        public async Task InvoicedAmountExceedsRequestAmount_Invalid()
        {
            var userId = "F8CF50DB-16C3-4545-B868-A7410020B01D";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(UserClaims.Id, userId) }));

            var token = CancellationToken.None;
            var userRepository = new Mock<IUserRepository>();

            userRepository.Setup(_ => _.GetUser(Guid.Parse(userId), token)).Returns(Task.FromResult(new User
            {
                Id = 56,
                State = UserState.Active
            }));

            var request = new Request()
            {
                Id = 42,
                State = RequestState.Approved,
                Amount = 10,
                User = new User { Id = 56 },
                Transactions = new[] { new Transaction() }
            };

            var requestRepository = new Mock<IRequestRepository>();

            requestRepository.Setup(_ => _.GetRequest(42, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(request));

            var unitOfWork = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<SetInvoicedAmountCommand>>();

            var command = new SetInvoicedAmountCommand(userRepository.Object, requestRepository.Object, unitOfWork.Object, loggerMock.Object);

            var exception = await Assert.ThrowsExceptionAsync<OperationErrorException>(() =>
                command.ExecuteAsync((42, new SetInvoicedAmountModel() { Amount = 99 }), user, token));

            Assert.AreEqual(ErrorCodes.InvalidAmount, exception.Code);
        }

        [TestMethod]
        public async Task RequestIsNotInApprovedSate_Invalid()
        {
            var userId = "F8CF50DB-16C3-4545-B868-A7410020B01D";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(UserClaims.Id, userId) }));

            var token = CancellationToken.None;
            var userRepository = new Mock<IUserRepository>();

            userRepository.Setup(_ => _.GetUser(Guid.Parse(userId), token)).Returns(Task.FromResult(new User
            {
                Id = 56,
                State = UserState.Active
            }));

            var request = new Request()
            {
                Id = 42,
                State = RequestState.Completed,
                Amount = 10,
                User = new User { Id = 56 },
                Transactions = new[] { new Transaction() }
            };

            var requestRepository = new Mock<IRequestRepository>();

            requestRepository.Setup(_ => _.GetRequest(42, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(request));

            var unitOfWork = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<SetInvoicedAmountCommand>>();

            var command = new SetInvoicedAmountCommand(userRepository.Object, requestRepository.Object, unitOfWork.Object, loggerMock.Object);

            var exception = await Assert.ThrowsExceptionAsync<OperationErrorException>(() =>
                command.ExecuteAsync((42, new SetInvoicedAmountModel() { Amount = 99 }), user, token));

            Assert.AreEqual(ErrorCodes.RequestNotApproved, exception.Code);
        }

    }
}