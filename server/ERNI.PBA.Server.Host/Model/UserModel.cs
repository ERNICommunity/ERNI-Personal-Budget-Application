public class UserModel
{
    public int Id { get; internal set; }
    public bool IsAdmin { get; internal set; }
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
    public SuperiorModel Superior { get; internal set; }
}