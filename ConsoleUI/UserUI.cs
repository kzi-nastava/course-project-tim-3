namespace HospitalSystem.ConsoleUI;

public abstract class UserUI : ConsoleUI
{
    protected User _user;

    protected UserUI(Hospital hospital, User user) : base(hospital)
    {
        _user = user;
    }
}