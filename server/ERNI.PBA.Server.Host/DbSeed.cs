using System;
using System.Linq;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host
{
    public class DbSeed
    {
        internal static void Seed(DatabaseContext context)
        {
            var names = new[] {
                "Emmanuel", "Walter", "Freddy", "Ho", "Bradleigh", "Cain", "Saskia", "Needham", "Betty", "Bautista",
                "Kain", "Caldwell", "Molly", "Parry", "Emelia", "Cresswell", "Bree", "Brook", "Kayla", "Woodley"
            };

            var categories = new[] {
                new RequestCategory { Title = "Sport" , IsActive = true},
                new RequestCategory { Title = "Education" , IsActive = true},
                new RequestCategory { Title = "Health" , IsActive = true},
                new RequestCategory { Title = "Other" , IsActive = true}
            };

            context.RequestCategories.AddRange(categories);

            context.SaveChanges();

            var users = Enumerable.Range(0, 10).Select(_ =>
            new DataAccess.Model.User
            {
                UniqueIdentifier = Guid.NewGuid().ToString(),

                IsAdmin = _ == 0,
                FirstName = names[_ * 2],
                LastName = names[_ * 2 + 1],
                Username = $"{names[_ * 2]}.{names[_ * 2 + 1]}",
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
                    Title = _.ToString(),
                    Amount = _ * 1878 % 50 + 10,
                    Date = new DateTime(budget.Year, _, 5),
                    Category = categories[_ % categories.Count()]
                }
                ));
            }

            context.SaveChanges();
        }
    }
}