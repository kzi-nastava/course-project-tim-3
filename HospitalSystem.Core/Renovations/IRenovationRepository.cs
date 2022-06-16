namespace HospitalSystem.Core.Renovations;

public interface IRenovationRepository
{
    public IQueryable<Renovation> GetAll();
   
    public void Insert(Renovation renovation);

    public void Replace(Renovation replacement);  // NOTE: expects existing!!
}