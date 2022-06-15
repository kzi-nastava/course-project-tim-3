namespace HospitalSystem.Core.DaysOff;
using HospitalSystem.Core.Medications.Requests;

public interface IDaysOffRepository
{
    public IQueryable<DaysOffRequest> GetAll();
    
    public void Upsert(DaysOffRequest newRequest);

    public void UpdateStatus(DaysOffRequest request, RequestStatus status);

    public void UpdateExplanation(DaysOffRequest request, string explanation);

    public IQueryable<DaysOffRequest> GetAllOnPending();
}