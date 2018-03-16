using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

public interface IUserRepository {
    Task<User> GetUser(int id);
}