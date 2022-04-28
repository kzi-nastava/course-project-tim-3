namespace Hospital;
public abstract class ConsoleUI
{
    protected Hospital _hospital;
    protected User? _user;

    public abstract void Start();

    public ConsoleUI(Hospital _hospital)
    {
        this._hospital = _hospital;
    }
}