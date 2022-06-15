using HospitalSystem.Core.Medications.Requests;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core.DaysOff;

public class DaysOffRequestService
{
    private IDaysOffRepository _repo;
    private AppointmentService _appointmentService;
    public DaysOffRequestService(IDaysOffRepository repo, AppointmentService appointmentService)
    {
        _repo = repo;
        _appointmentService = appointmentService;
    }

    public void Send(DaysOffRequest request)
    {
        request.Status = RequestStatus.SENT;
        _repo.Upsert(request);
    }

    public void Resend(DaysOffRequest request)
    {
        request.Status = RequestStatus.SENT;
        _repo.Upsert(request);
    }

    public void Deny(DaysOffRequest request)
    {
        request.Status = RequestStatus.DENIED;
        _repo.Upsert(request);
    }

    public void Approve(DaysOffRequest request)
    {
        request.Status = RequestStatus.APPROVED;
        _repo.Upsert(request);
    }

    public void ApproveUrgent(DaysOffRequest request)
    {
        foreach (DateTime day in request.DaysOff.EachDay())
        {
            List<Checkup> checkups = _appointmentService.GetCheckupsByDay(day);
            List<Operation> operations = _appointmentService.GetOperationsByDay(day);
            foreach (Checkup checkup in checkups)
                _appointmentService.DeleteCheckup(checkup);
            foreach (Operation operation in operations)
                _appointmentService.DeleteOperation(operation);
        }
        request.Status = RequestStatus.APPROVED;
        _repo.Upsert(request);
    }


    public bool DaysOffAllowed(DateRange daysOff)
    {
        if (daysOff.Starts < DateTime.Today.AddDays(2)) 
            return false;

        foreach(DateTime day in daysOff.EachDay())
        {  
            if (_appointmentService.GetCheckupsByDay(day).Count() != 0)
            {
                return false;
            }
            if (_appointmentService.GetOperationsByDay(day).Count() != 0)
            {
                return false;
            }
        }
        return true;
    }

    public IQueryable<DaysOffRequest> GetAll()
    {
        return _repo.GetAll();
    }

    public IQueryable<DaysOffRequest> GetAllOnPending()
    {
        return _repo.GetAllOnPending();
    }

    public void UpdateStatus(DaysOffRequest request, RequestStatus status)
    {
        _repo.UpdateStatus(request, status);
    }

    public void UpdateExplanation(DaysOffRequest request, string explanation)
    {
        _repo.UpdateExplanation(request, explanation);
    }
}