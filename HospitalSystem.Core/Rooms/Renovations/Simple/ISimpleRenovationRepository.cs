namespace HospitalSystem.Core;

public interface ISimpleRenovationRepository
{
    public IQueryable<SimpleRenovation> GetAll();

    public void Insert(SimpleRenovation renovation);

    public void Replace(SimpleRenovation replacing);  // NOTE: expects existing!!
}