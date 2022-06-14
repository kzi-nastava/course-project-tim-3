using HospitalSystem.Core.Medications.Requests;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public class DaysOffRequestService
{
    private DaysOffRequestRepository _repo;
    private AppointmentService _appointmentService;
    public DaysOffRequestService(DaysOffRequestRepository repo, AppointmentService appointmentService)
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
}