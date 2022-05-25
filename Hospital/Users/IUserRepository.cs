namespace HospitalSystem;

public interface IUserRepository
{
    public IQueryable<User> GetAll();

    public User Get(string email);

    public void Upsert(User user);

    public bool Delete(string email);
}