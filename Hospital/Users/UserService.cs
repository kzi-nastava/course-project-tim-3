namespace HospitalSystem;

[System.Serializable]
public class UserDoesNotExistException : System.Exception
{
    public UserDoesNotExistException() { }
    public UserDoesNotExistException(string message) : base(message) { }
    public UserDoesNotExistException(string message, System.Exception inner) : base(message, inner) { }
    protected UserDoesNotExistException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class UserService
{
    private IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public IQueryable<User> GetAll()
    {
        return _repo.GetAll();
    }

    public User Get(string email)
    {
        return _repo.Get(email);
    }

    public void BlockPatient(string email)
    {
        var userToBlock = _repo.Get(email);
        if (userToBlock.Role != Role.PATIENT)
        {
            throw new UserDoesNotExistException("User " + email + " is not a patient.");
        }
        if (userToBlock.BlockStatus != Block.UNBLOCKED){
            throw new UserDoesNotExistException("User " + email + " is already blocked.");
        }
        userToBlock.BlockStatus = Block.BY_SECRETARY;

        _repo.Upsert(userToBlock);
    }

    public IQueryable<User> GetAllBlocked()
    {
        return 
            from user in _repo.GetAll()
            where user.BlockStatus != Block.UNBLOCKED
            select user;
    }

    public void UnblockPatient(string email)
    {
        var userToUnblock = _repo.Get(email);
        userToUnblock.BlockStatus = Block.UNBLOCKED;
        _repo.Upsert(userToUnblock);
    }

    public User? Login(string email, string password)
    {
        var users = _repo.GetAll();
        var matchingUsers = 
            from user in users.AsQueryable()
            where user.Password == password && user.Email == email
            select user;
        // count on database that there is only one with this email
        return matchingUsers.FirstOrDefault();
    }

    public void Upsert(User user)  // DOES NOT CHECK IF EMAIL IS TAKEN!!
    {
        _repo.Upsert(user);
    }

    public void UpdatePassword(string email, string newPassword)
    {
        var newUser = _repo.Get(email);
        newUser.Password = newPassword;
        _repo.Upsert(newUser);  // TODO: don't upsert for updates
    }

    public void UpdateEmail(string email, string newEmail)
    {
        var newUser = _repo.Get(email);
        newUser.Email = newEmail;
        _repo.Upsert(newUser);
    }

    public void Delete(string email)
    {
        var success = _repo.Delete(email);
        if (!success)
            throw new UserDoesNotExistException("User " + email + " does not exist.");
    }
}
