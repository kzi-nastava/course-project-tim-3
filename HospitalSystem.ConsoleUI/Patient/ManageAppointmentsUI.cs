using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;
//TODO: CHANGE THIS 
public class ManageAppointmentsUI : PatientUI
{
    public ManageAppointmentsUI(Hospital hospital, User user) : base(hospital, user) 
    {
        _loggedInPatient = _hospital.PatientService.GetById((ObjectId) user.Person.Id);
    }

    public override void Start()
    {
        while (true){
            System.Console.WriteLine(@"
            Commands:
            cc - create checkup
            ccr - create checkup (with recommendations)
            va - view and manage appointments
            return - go to the previous menu
            exit - quit the program

            ");

            string selectedOption = ReadSanitizedLine().Trim();
           
            if (selectedOption == "cc")
            {
                CreateCheckup();
            }
            else if (selectedOption == "ccr")
            {
                CreateCheckupAdvanced();
            }
            else if (selectedOption == "va")
            {
                AppointmentRUDUI ui = new(_hospital, _user);
                ui.Start();
            }
            else if (selectedOption == "return")
            {
                Console.WriteLine("Returning...\n");
                break;
            }
            else if (selectedOption == "exit")
            {
                Console.WriteLine("Exiting...\n");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Unrecognized command, please try again");
            }
           
        }
    }

    public void DeleteCheckup ()
    {
        bool nextWillBlock = _hospital.PatientService.WillNextCRUDOperationBlock(CRUDOperation.DELETE, _loggedInPatient);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup deletion will result in account block!");
        }
        Checkup selectedCheckup;
        try
        {   
            ShowCheckups(AppointmentInTime.FUTURE);
            selectedCheckup = SelectCheckup();
        }
        catch (QuitToMainMenuException)
        {
            return;
        }

        if (selectedCheckup.DateRange.Starts < DateTime.Now.AddDays(2))
        {
            CheckupChangeRequest newRequest = new CheckupChangeRequest(
                selectedCheckup,
                CRUDOperation.DELETE);
                Console.WriteLine("Checkup date is in less than 2 days from now. Change request sent.");
                _hospital.CheckupChangeRequestService.Upsert(newRequest);
        }
        else
        {
            _hospital.AppointmentService.DeleteCheckup(selectedCheckup);
            Console.WriteLine("Checkup deleted.");
        }

        _hospital.PatientService.LogChange(CRUDOperation.DELETE,_loggedInPatient);
        if (nextWillBlock)
        {
            _hospital.UserService.BlockUser(_user);
            throw new UserBlockedException("Deleting too many checkups.");
        }

    }
    public Doctor ChangeDoctor(Doctor currentDoctor)
    {

        List<Doctor> alternativeDoctors =  _hospital.DoctorService.GetManyBySpecialty(currentDoctor.Specialty);
        alternativeDoctors.Remove(currentDoctor);

            if (alternativeDoctors.Count == 0)
            {
                Console.WriteLine("No doctors found in the same specialty.");
                return currentDoctor;
            }

            for (int i=0; i<alternativeDoctors.Count; i++)
            {
                Console.WriteLine(i+" - "+alternativeDoctors[i].ToString());
            }

            int selectedDoctorIndex = -1;
            
            System.Console.Write("Please enter a number from the list: ");
            selectedDoctorIndex = ReadInt(0, alternativeDoctors.Count-1);
            
            return alternativeDoctors[selectedDoctorIndex];
    }

    public void UpdateCheckup()
    {

        bool nextWillBlock = _hospital.PatientService.WillNextCRUDOperationBlock(CRUDOperation.UPDATE, _loggedInPatient);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup updating will result in account block!");
        }
        Checkup selectedCheckup;
        try
        {
            ShowCheckups(AppointmentInTime.FUTURE);
            selectedCheckup = SelectCheckup();
        }
        catch (QuitToMainMenuException)
        {
            return;
        }
        Console.WriteLine ("You have selected " + ConvertAppointmentToString(selectedCheckup));

        Doctor currentDoctor = _hospital.DoctorService.GetById((ObjectId)selectedCheckup.Doctor.Id);
        DateTime existingDate = selectedCheckup.DateRange.Starts;
        Doctor newDoctor = currentDoctor;
        DateTime newDate = existingDate;
        
        Console.WriteLine("Change doctor? Enter yes or no: ");
        string changeDoctorOpinion = ReadSanitizedLine().Trim();

        if (changeDoctorOpinion !="yes" && changeDoctorOpinion!="no")
        {
            Console.WriteLine("Wrong command. Aborting...");
            return;
        }

        if (changeDoctorOpinion == "yes")
        {
            try
            {
                newDoctor = ChangeDoctor(currentDoctor);
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Aborting...");
                return;
            }
        }

        Console.WriteLine("Change date? Enter yes or no: ");

        string changeDateOpinion =ReadSanitizedLine().Trim().ToLower();

        if (changeDateOpinion !="yes" && changeDateOpinion!="no")
        {
            Console.WriteLine("Wrong command. Aborting...");
            return;
        }

        if (changeDateOpinion == "yes")
        {
            newDate = SelectDateAndTime();
            Console.WriteLine("You have selected the following date - "+ newDate);
        }

        selectedCheckup.Doctor = new MongoDB.Driver.MongoDBRef("doctors", newDoctor.Id);
        DateTime oldDate = selectedCheckup.DateRange.Starts;
        selectedCheckup.DateRange = new DateRange(newDate, newDate.Add(Checkup.DefaultDuration), allowPast: false);
        
        if (!_hospital.ScheduleService.IsDoctorAvailable(selectedCheckup.DateRange, newDoctor))
        {
            Console.WriteLine("Checkup already taken.");
            return;
        }
        
        if (oldDate < DateTime.Now.AddDays(2))
        {
            CheckupChangeRequest newRequest = new CheckupChangeRequest(
                selectedCheckup,
                CRUDOperation.UPDATE);
            Console.WriteLine("Checkup date is in less than 2 days from now. Change request sent.");
            _hospital.CheckupChangeRequestService.Upsert(newRequest);
        }
        else
        {
            _hospital.AppointmentService.UpsertCheckup(selectedCheckup);
            Console.WriteLine("Checkup updated.");
        }
        
        _hospital.PatientService.LogChange(CRUDOperation.UPDATE,_loggedInPatient);
        if (nextWillBlock)
        {
            _hospital.UserService.BlockUser(_user);
            throw new UserBlockedException("Updating too many checkups.");
        }

    }

    public void CreateCheckupAdvanced()
    {
        bool nextWillBlock = _hospital.PatientService.WillNextCRUDOperationBlock(CRUDOperation.CREATE, _loggedInPatient);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup creation will result in account block!");
        }

        Specialty selectedSpecialty;
        try
        {
            selectedSpecialty = SelectSpecialty();
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return;
        }

        Doctor? selectedSuitableDoctor = SelectDoctor(selectedSpecialty);
        if (selectedSuitableDoctor == null)
        {
            return;
        }

        //TODO: this doesnt have to be in 15 minute slots
        System.Console.WriteLine("Please select starting time");
        DateTime intervalStart = SelectTime(new DateTime());
        System.Console.WriteLine("Please select starting time");
        DateTime intervalEnd = SelectTime(new DateTime());
        Console.Write("Please enter a date in dd-MM-yyyy format: ");
        DateTime deadline = SelectDate();

        if (intervalStart >= intervalEnd)
        {
            System.Console.WriteLine("Wrong start and end time. Aborting...");
            return;
        }

        List<Checkup> recommendedCheckups;

        Console.Write("Is time interval a priority? Enter y if yes, anything else for doctor: ");
        string choice = ReadSanitizedLine().Trim();
        bool isIntervalPriority = false;

        if (choice == "y")
        {
            isIntervalPriority = true;
        }

        DateRange interval = new DateRange(intervalStart,intervalEnd, true);
        recommendedCheckups = _hospital.ScheduleService.FindSuitableCheckups(selectedSuitableDoctor,interval,deadline,isIntervalPriority,_user);
        
        if (recommendedCheckups.Count == 1)
        {
            Checkup result = recommendedCheckups[0];
            Console.WriteLine("Recommendation:");
            Doctor referencedDoctor = _hospital.DoctorService.GetById((ObjectId)result.Doctor.Id);
            Console.WriteLine(referencedDoctor.ToString()+" "+result.DateRange.Starts);

            Console.Write("Create checkup? Enter y for yes: ");
            if (ReadSanitizedLine().Trim() == "y")
            {
                _hospital.AppointmentService.UpsertCheckup(result);
                Console.WriteLine("Checkup created.");
                
                _hospital.PatientService.LogChange(CRUDOperation.CREATE,_loggedInPatient);
                if (nextWillBlock)
                {
                    _hospital.UserService.BlockUser(_user);
                    throw new UserBlockedException("Creating too many checkups.");
                }

                return;
            }
            Console.WriteLine("Checkup creation canceled.");

        }
        else
        {
            Console.WriteLine("Recommendations:");
            for (int i=0; i<recommendedCheckups.Count; i++)
            {
                Checkup result = recommendedCheckups[i];
                Doctor referencedDoctor = _hospital.DoctorService.GetById((ObjectId)result.Doctor.Id);
                Console.WriteLine(i+" - "+referencedDoctor.ToString()+" "+result.DateRange.Starts);
            }

            System.Console.Write("Please enter a number from the list: ");
            int selectedIndex;
            try
            {
                selectedIndex = ReadInt(0, 2);
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Aborting...");
                return;
            }
            _hospital.AppointmentService.UpsertCheckup(recommendedCheckups[selectedIndex]);
            Console.WriteLine("Checkup created.");
                
            _hospital.PatientService.LogChange(CRUDOperation.CREATE,_loggedInPatient);
            if (nextWillBlock)
            {
                _hospital.UserService.BlockUser(_user);
                throw new UserBlockedException("Creating too many checkups.");
            }
        }
    }
}