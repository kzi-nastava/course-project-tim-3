namespace HospitalSystem.Core;

public interface IMedicationRequestRepository
{
    public void Insert(MedicationRequest request);

    public void Replace(MedicationRequest request);

    public IQueryable<MedicationRequest> GetAll();
}