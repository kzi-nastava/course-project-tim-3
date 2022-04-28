namespace Hospital;
public abstract class ConsoleUI
{
    protected Hospital _hospital;
    protected User? _user;

    public abstract void Start();

    public ConsoleUI(Hospital hospital)
    {
        this._hospital = hospital;
    }
}