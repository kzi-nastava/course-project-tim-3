namespace HospitalSystem.Core.Medications.Requests;

public interface IMedicationRequestRepository
{
    public void Insert(MedicationRequest request);

    public void Replace(MedicationRequest request);

    public IQueryable<MedicationRequest> GetAll();

    public IQueryable<MedicationRequest> GetSent();

    public IQueryable<MedicationRequest> GetDenied();
}