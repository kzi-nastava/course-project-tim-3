namespace HospitalSystem;

public interface ISplitRenovationRepository
{
    public IQueryable<SplitRenovation> GetAll();
   
    public void Insert(SplitRenovation renovation);

    public void Replace(SplitRenovation replacing);  // NOTE: expects existing!!
}