using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Import
{
    public class Importer
    {
        public static void Import(DatabaseContext context)
        {
            using (SqlConnection con = new SqlConnection("CONNSTR"))
            {
                con.Open();

                var oldEmployees = GetEmployees(con);
                var oldBudgets = GetBudgets(con);
                var oldRequests = GetRequests(con);
                var oldOccasions = GetOccasions(con);

                var newEmployees = oldEmployees.Select(_ => new
                {
                    OldId = _.id,
                    User = new User
                    {
                        FirstName = _.firstName,
                        LastName = _.lastName,
                        IsAdmin = _.isAdmin,
                        Username = _.shortName + "@" + _.mail.Split('@')[1]
                    }
                }).ToArray();

                context.Users.AddRange(newEmployees.Select(_ => _.User));

                context.SaveChanges();

                var budgets = oldBudgets.Select(_ => new
                {
                    OldId = _.id,
                    Budget = new Budget
                    {
                        UserId = newEmployees.Single(e => e.OldId == _.employeeId).User.Id,
                        Year = _.year,
                        Amount = _.amount
                    }
                }).ToArray();

                context.Budgets.AddRange(budgets.Select(_ => _.Budget));

                context.SaveChanges();

                var requests = oldRequests.Select(_ => new
                {
                    OldId = _.id,
                    Request = new Request
                    {
                        Year = budgets.Single(e => e.OldId == _.budgetId).Budget.Year,
                        UserId = budgets.Single(e => e.OldId == _.budgetId).Budget.UserId,
                        Title = _.title,
                        Amount = _.amount
                    }
                }).ToArray();

                context.Requests.AddRange(requests.Select(_ => _.Request));
            }

            context.SaveChanges();
        }

        private static (int id, int employeeId, int year, decimal amount)[] GetBudgets(SqlConnection con)
        {
            string sqlQuery = "SELECT * FROM Budgets";
            using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
            {

                // con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                var l = new List<(int id, int employeeId, int year, decimal amount)>();

                while (rdr.Read())
                {
                    var employee =
                    (
                        id: Convert.ToInt32(rdr["ID"]),
                        employeeId: Convert.ToInt32(rdr["EmployeeID"]),
                        year: Convert.ToInt32(rdr["Year"]),
                        amount: Convert.ToDecimal(rdr["AllowedAmount"])
                    );

                    l.Add(employee);
                }
                rdr.Close();
                return l.ToArray();
            }
        }

        private static (int id, string firstName, string lastName, string shortName, string mail, bool isAdmin)[] GetEmployees(SqlConnection con)
        {
            string sqlQuery = "SELECT * FROM Employees";
            using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
            {

                // con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                var l = new List<(int id, string firstName, string lastName, string shortName, string mail, bool isAdmin)>();

                while (rdr.Read())
                {
                    var employee =
                    (
                        id: Convert.ToInt32(rdr["ID"]),
                        firstName: rdr["FirstName"].ToString(),
                        lastName: rdr["LastName"].ToString(),
                        shortName: rdr["ShortName"].ToString(),
                        mail: rdr["Mail"].ToString(),
                        isAdmin: Convert.ToBoolean(rdr["IsAdmin"])
                    );

                    l.Add(employee);
                }
                rdr.Close();
                return l.ToArray();
            }
        }

        private static (int id, string title)[] GetOccasions(SqlConnection con)
        {
            string sqlQuery = "SELECT * FROM Occasions";
            using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
            {

                // con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                var l = new List<(int id, string title)>();

                while (rdr.Read())
                {
                    var employee =
                    (
                        id: Convert.ToInt32(rdr["ID"]),
                        title: rdr["Title"].ToString()
                    );

                    l.Add(employee);
                }
                rdr.Close();
                return l.ToArray();
            }
        }

        private static (int id, string title, DateTime date, decimal amount, int requestState, bool isClosed, int occasionId, int budgetId)[] GetRequests(SqlConnection con)
        {
            string sqlQuery = "SELECT * FROM Requests";
            using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
            {

                // con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                var l = new List<(int id, string title, DateTime date, decimal amount, int requestState, bool isClosed, int occasionId, int budgetId)>();

                while (rdr.Read())
                {
                    var employee =
                    (
                        id: Convert.ToInt32(rdr["ID"]),
                        title: rdr["Title"].ToString(),
                        date: Convert.ToDateTime(rdr["Date"]),
                        amount: Convert.ToDecimal(rdr["Amount"]),
                        requestState: Convert.ToInt32(rdr["RequestState"]),
                        isClosed: Convert.ToBoolean(rdr["IsClosed"]),
                        occasionId: Convert.ToInt32(rdr["OccasionId"]),
                        budgetId: Convert.ToInt32(rdr["BudgetId"])
                    );

                    l.Add(employee);
                }
                rdr.Close();
                return l.ToArray();
            }
        }
    }
}
