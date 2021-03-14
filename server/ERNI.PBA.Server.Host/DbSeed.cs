using System;
using System.Globalization;
using System.Linq;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Host
{
    public static class DbSeed
    {
        internal static void Seed(DatabaseContext context) => SeedUsers(context);

        private static void SeedUsers(DatabaseContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            var names = new[]
            {
                "Emmanuel", "Walter", "Freddy", "Ho", "Bradleigh", "Cain", "Saskia", "Needham", "Betty", "Bautista",
                "Kain", "Caldwell", "Molly", "Parry", "Emelia", "Cresswell", "Bree", "Brook", "Kayla", "Woodley"
            };

            var categories = new[]
            {
                new RequestCategory { Title = "Sport", IsActive = true, IsUrlNeeded = false, SpendLimit = null },
                new RequestCategory { Title = "Education", IsActive = true, IsUrlNeeded = false, SpendLimit = null },
                new RequestCategory { Title = "Health", IsActive = true, IsUrlNeeded = true, SpendLimit = 200 },
                new RequestCategory { Title = "Other", IsActive = true, IsUrlNeeded = false, SpendLimit = null }
            };

            context.RequestCategories.AddRange(categories);

            context.SaveChanges();

            var users = Enumerable.Range(0, 10).Select(_ =>
            new User
            {
                UniqueIdentifier = Guid.NewGuid().ToString(),

                IsAdmin = _ == 0,
                IsSuperior = _ == 0,
                IsViewer = _ == 0,
                FirstName = names[_ * 2],
                LastName = names[(_ * 2) + 1],
                Username = $"{names[_ * 2]}.{names[(_ * 2) + 1]}",
                SuperiorId = null,
                State = (UserState)(_ % 3)
            }).ToArray();

            context.Users.AddRange(users);

            context.SaveChanges();

            foreach (var user in context.Users.Where(u => u.Id != 1))
            {
                user.SuperiorId = 1;
            }

            context.SaveChanges();

            foreach (var user in users)
            {
                context.Budgets.Add(new Budget
                {
                    User = user,
                    Year = 2017,
                    Amount = 300
                });

                context.Budgets.Add(new Budget
                {
                    User = user,
                    Year = 2018,
                    Amount = 350
                });
            }

            context.SaveChanges();

            var budgets = context.Budgets.ToArray();

            foreach (var budget in budgets)
            {
                context.Requests.AddRange(Enumerable.Range(1, 10).Select(_ =>
                new Request
                {
                    Budget = budget,
                    Title = _.ToString(CultureInfo.InvariantCulture),
                    Amount = (_ * 1878 % 50) + 10,
                    Date = new DateTime(budget.Year, _, 5),
                    Category = categories[_ % categories.Length]
                }));
            }

            context.SaveChanges();
        }
    }
}