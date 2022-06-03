namespace HospitalSystem.Core;

public interface IMergeRenovationRepository
{
    public IQueryable<MergeRenovation> GetAll();

    public void Insert(MergeRenovation renovation);

    public void Replace(MergeRenovation replacing);  // NOTE: expects existing!!
}