using System.Linq;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERNI.PBA.Server.Business.UnitTests.Utils
{
    [TestClass]
    public class TransactionCalculatorTests
    {
        [TestMethod]
        public void UnevenDistributionTest()
        {
            var budgets = new[]
            {
                CreateTeamBudget(1, 200),
                CreateTeamBudget(2, 100),
                CreateTeamBudget(3, 300),
                CreateTeamBudget(4, 400),
            };

            var result = TransactionCalculator.Create(budgets, 800);

            Assert.IsTrue(result.Any(_ => _.BudgetId == 1 && _.Amount == 200));
            Assert.IsTrue(result.Any(_ => _.BudgetId == 2 && _.Amount == 100));
            Assert.IsTrue(result.Any(_ => _.BudgetId == 3 && _.Amount == 250));
            Assert.IsTrue(result.Any(_ => _.BudgetId == 4 && _.Amount == 250));
        }

        [TestMethod]
        public void SingleBudgetTest()
        {
            var budgets = new[]
            {
                CreateTeamBudget(1, 200),
            };

            var result = TransactionCalculator.Create(budgets, 800).Single();

            Assert.AreEqual(1, result.BudgetId);
            Assert.AreEqual(200, result.Amount);
        }

        private static TeamBudget CreateTeamBudget(int id, decimal amount)
        {
            return new()
            {
                BudgetId = id,
                UserId = id,
                Amount = amount
            };
        }
    }
}