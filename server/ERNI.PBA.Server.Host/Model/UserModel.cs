using ERNI.PBA.Server.DataAccess.Model;

public class UserModel
{
    public int Id { get; internal set; }
    public bool IsAdmin { get; internal set; }
    public bool IsSuperior { get; internal set; }
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
    public SuperiorModel Superior { get; internal set; }
    public UserState State { get; internal set; }
}