namespace HospitalSystem.Core;

public class PersonService
{
    private IPersonRepository _repo;

    public PersonService(IPersonRepository repo)
    {
        _repo = repo;
    }

    public void Insert(Person person)
    {
        _repo.Insert(person);
    }
}