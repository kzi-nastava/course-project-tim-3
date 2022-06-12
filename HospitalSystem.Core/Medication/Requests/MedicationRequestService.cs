namespace HospitalSystem.Core;

public class MedicationRequestService
{
    private IMedicationRequestRepository _repo;
    private MedicationRepository _medicationRepo;  // TODO: should this be in thic class as field?

    public MedicationRequestService(IMedicationRequestRepository repo, MedicationRepository medicationRepo)
    {
        _repo = repo;
        _medicationRepo = medicationRepo;
    }

    public void Send(MedicationRequest request)
    {
        request.Status = RequestStatus.SENT;
        _repo.Insert(request);
    }

    public void Resend(MedicationRequest request)
    {
        request.Status = RequestStatus.SENT;
        _repo.Replace(request);
    }

    public void Deny(MedicationRequest request)
    {
        request.Status = RequestStatus.DENIED;
        _repo.Replace(request);
    }

    public void Approve(MedicationRequest request)
    {
        request.Status = RequestStatus.APPROVED;
        _medicationRepo.AddOrUpdate(request.Requested);
        _repo.Replace(request);
    }

    public IQueryable<MedicationRequest> GetDenied()
    {
        return _repo.GetDenied();
    }

    public IQueryable<MedicationRequest> GetSent()
    {
        return _repo.GetSent();
    }
}