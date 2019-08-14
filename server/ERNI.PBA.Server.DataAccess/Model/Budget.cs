using ERNI.PBA.Server.DataAccess.Model;

public class Budget
{
    public int Year { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public virtual User User { get; set; }
}