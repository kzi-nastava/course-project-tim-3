using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using HospitalSystem.Core;
using HospitalSystem.Core.Rooms;
using HospitalSystem.Core.Equipment;
using HospitalSystem.Core.Surveys;
using HospitalSystem.Core.Utils;

public static class TestGenerator
{
    // THIS WILL DELETE YOUR EXISTING DATABASE!!
    public static void Generate(Hospital hospital)
    {
        // TODO: split to functions
        var dbClient = new MongoClient("mongodb://root:root@localhost:27017");  // TODO: unhardcode
        dbClient.DropDatabase("hospital");
        System.Console.WriteLine("DROPPED EXISTING DATABASE HOSPITAL");

        GenerateUsers(hospital);
        GenerateRoomsAndEquipment(hospital);
        GenerateCheckupsAndOperations(hospital);
        GenerateCheckupChangeRequests(hospital);
        GenerateMedication(hospital);
        GenerateMedicationRequests(hospital);
        GenerateSurveys(hospital);

        System.Console.WriteLine("GENERATED TESTS IN DB");

        WriteDatabaseToFile(dbClient);

        System.Console.WriteLine("WROTE TESTS TO FILE");
    }

    private static void GenerateCheckupChangeRequests(Hospital hospital)
    {
        for (int i = 0; i < 20; i++)
        {
            Doctor doctor = hospital.DoctorService.GetByFullName("name1","surname1");
            List<Checkup> checkups = hospital.AppointmentService.GetCheckupsByDoctor(doctor.Id);

            if (i % 2 == 0)
            {   RequestState state = RequestState.PENDING;
                if (i % 4 == 0)
                {
                    state = RequestState.APPROVED;
                }
                Checkup alteredCheckup = checkups[i];
                DateTime newDateAndTime =  new DateTime(2077,10,10);
                alteredCheckup.DateRange = new DateRange(newDateAndTime, newDateAndTime.Add(Checkup.DefaultDuration), true);
                CheckupChangeRequest request = new CheckupChangeRequest(alteredCheckup,CRUDOperation.UPDATE,state);
                hospital.CheckupChangeRequestService.AddOrUpdate(request);
            }
            else if (i % 2 == 1) 
            {
                RequestState state = RequestState.PENDING;
                if (i % 3 == 0)
                {
                    state = RequestState.DENIED;
                }
                CheckupChangeRequest request = new CheckupChangeRequest(checkups[i],CRUDOperation.DELETE,state);
                hospital.CheckupChangeRequestService.AddOrUpdate(request);
            }    
        }
    }

    private static void GenerateRoomsAndEquipment(Hospital hospital)
    {
        for (int i = 0; i < 10; i++)
        {
            if (i % 3 == 0)
            {   
                var newRoom = new Room("90" + i, "NA" + i, RoomType.STOCK);
                hospital.RoomService.Insert(newRoom);
                for (int j = 0; j < 4; j++)
                {
                    var newEquipmentBatch = new EquipmentBatch(newRoom.Location, "scalpel", 3, EquipmentType.OPERATION);
                    hospital.EquipmentService.Add(newEquipmentBatch);
                }
            } 
            else if (i % 3 == 1)
            {
                var newRoom = new Room("10" + i, "NA" + i, RoomType.OPERATION);
                hospital.RoomService.Insert(newRoom);
            } 
            else
            {
                var newRoom = new Room("55" + i, "NA" + i, RoomType.CHECKUP);
                hospital.RoomService.Insert(newRoom);
                var newEquipmentBatch = new EquipmentBatch(newRoom.Location, "syringe", 10, EquipmentType.CHECKUP);
                hospital.EquipmentService.Add(newEquipmentBatch);
                var newEquipmentBatch2 = new EquipmentBatch(newRoom.Location, "bandage", 10, EquipmentType.CHECKUP);
                hospital.EquipmentService.Add(newEquipmentBatch2);
            }
        }
    }

    private static void GenerateCheckupsAndOperations(Hospital hospital)
    {
        DateTime dateTime = new DateTime(2022, 6, 3, 4, 15, 0);
        int i = 0;
        try
        {
            for (; i < 100; i++)
            {
                Doctor doctor = hospital.DoctorService.GetByFullName("name1","surname1");
                Patient patient = hospital.PatientService.GetPatientByFullName("name2","surname2");
                dateTime = dateTime.AddHours(1);

                if (i % 2 == 0)
                {   
                    // doing this to allow writing to the past
                    var range = new DateRange(dateTime, dateTime.Add(Checkup.DefaultDuration), allowPast: true);
                    Random rand = new Random();
                    Checkup check = new Checkup(range, new MongoDBRef("patients",patient.Id),
                        new MongoDBRef("doctors", doctor.Id), "anamneza");
                    hospital.AppointmentService.UpsertCheckup(check);
                } else if (i % 2 == 1) 
                {
                    Random rand = new Random();
                    var range = new DateRange(dateTime, dateTime.Add(new TimeSpan(1, 15, 0)), allowPast: true);
                    Operation op = new Operation(range, new MongoDBRef("patients",patient.Id),
                        new MongoDBRef("doctors", doctor.Id), "report");
                    hospital.AppointmentService.UpsertOperation(op);
                }
            }
        }
        catch (NoAvailableRoomException e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine("Stopping appointment generation at " + i + " generated");
        }
    }

    private static void GenerateUsers(Hospital hospital)
    {
        int doctorSpecialtynumber = 0;
        for (int i = 0; i < 100; i++)
        {
            User user;
            if (i % 4 == 0)
            {
                var director = new Director("name" + i, "surname" + i);
                user = new User("a" + i, "a" + i, director, Role.DIRECTOR);
                hospital.DirectorRepo.AddOrUpdateDirector(director);
            }
            else if (i % 4 == 1)
            {
                int namesCount = Enum.GetNames(typeof(Specialty)).Length;
                // counts on derma being first
                Specialty doctorsSpecialty = (Specialty)(doctorSpecialtynumber%namesCount+Specialty.DERMATOLOGY);
                doctorSpecialtynumber++; 
                var doctor = new Doctor("name" + i,"surname" + i, doctorsSpecialty);
                user = new User("a" + i, "a" + i, doctor, Role.DOCTOR);
                hospital.DoctorService.Upsert(doctor);
            }
            else if (i % 4 == 2) 
            {
                var patient = new Patient("name" + i, "surname" + i, new MedicalRecord());
                hospital.PatientService.AddOrUpdatePatient(patient);
                user = new User("a" + i, "a" + i, patient, Role.PATIENT);
            }  
            else
            {
                var secretary = new Secretary("name" + i, "surname" + i);
                user = new User("a" + i, "a" + i, secretary, Role.SECRETARY);
                hospital.SecretaryRepo.AddOrUpdateSecretary(secretary);
            }
            hospital.UserService.Upsert(user);                
        }
    }

    private static void GenerateMedication(Hospital hospital)
    {
        hospital.MedicationRepo.AddOrUpdate(new Medication("ibuprofen", new List<string> {"lactose", "Maize Starch", "Hypromellose", "sodium starch glycollate", "colloidal Anhydrous Silica", "magnesium Stearate", "sucrose", "talc", "titanium Dioxide (E171)", "carnauba Wax"}));
        hospital.MedicationRepo.AddOrUpdate(new Medication("probiotic", new List<string> {"lactobacillus"}));
        hospital.MedicationRepo.AddOrUpdate(new Medication("amoxicillin", new List<string> {"penicillin","magnesium Stearate (E572)", "Colloidal Anhydrous Silica"}));
        hospital.MedicationRepo.AddOrUpdate(new Medication("oxacillin", new List<string> {"penicillin"}));
    }

    private static void GenerateMedicationRequests(Hospital hospital)
    {
        hospital.MedicationRequestService.Send(new MedicationRequest(
            new Medication("ultra probiotic", new List<string> {"ultra lactobacillus"}), "ULTRA"));
        hospital.MedicationRequestService.Send(new MedicationRequest(
            new Medication("ultra amoxicillin", new List<string> {"ultra penicillin","mega magnesium Stearate (E572)", "Colloidal Anhydrous Silica"}), "ULTRA2"));
        hospital.MedicationRequestService.Send(new MedicationRequest(
            new Medication("ultra oxacillin", new List<string> {"ultra penicillin"}), "ULTRA3"));
    }

    private static void GenerateSurveys(Hospital hospital)
    {
        var hospitalSurvey = new HospitalSurvey(new List<string> {"Opininion?"},
            new List<string>{"Overall"}, "Hospital1");
        hospital.SurveyService.Insert(hospitalSurvey);
        hospital.SurveyService.AddResponse(hospitalSurvey,
            new SurveyResponse(new List<string?>{null}, new List<int?>{4},
            hospital.PatientService.GetPatientByFullName("name2", "surname2").Id));
        hospital.SurveyService.AddResponse(hospitalSurvey,
            new SurveyResponse(new List<string?>{"Bad hospital! Hate it!"}, new List<int?>{5},
            hospital.PatientService.GetPatientByFullName("name6", "surname6").Id));

        var doctorSurvey = new DoctorSurvey(new List<string> {"Opininion?"},
            new List<string>{"Overall"}, "Doctor1");
        hospital.SurveyService.Insert(doctorSurvey);
        hospital.SurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"Good good"}, new List<int?>{null},
                hospital.PatientService.GetPatientByFullName("name2", "surname2").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.STOMATOLOGY));
        hospital.SurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"Very Good good"}, new List<int?>{4},
                hospital.PatientService.GetPatientByFullName("name6", "surname6").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.DERMATOLOGY));
        hospital.SurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"Very much Good good"}, new List<int?>{3},
                hospital.PatientService.GetPatientByFullName("name10", "surname10").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.DERMATOLOGY));
        hospital.SurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"BAD Doctor"}, new List<int?>{1},
                hospital.PatientService.GetPatientByFullName("name2", "surname2").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.RADIOLOGY));

        hospital.SurveyService.Insert(new HospitalSurvey(new List<string> {"Opininion2?"},
            new List<string>{"Overall2"}, "Hospital2"));
        hospital.SurveyService.Insert(new DoctorSurvey(new List<string> {"Opininion2?"},
            new List<string>{"Overall2"}, "Doctor2"));
    }

    private static void WriteDatabaseToFile(MongoClient dbClient)
    {
        Dictionary<String, List<Object>> allCollections = new();
        foreach (string collectionName in dbClient.GetDatabase("hospital").ListCollectionNames().ToEnumerable())
        {
            var collection = dbClient.GetDatabase("hospital").GetCollection<BsonDocument>(collectionName).AsQueryable();
            allCollections[collectionName] = new();
            allCollections[collectionName].AddRange(collection);
        }
        File.WriteAllText("db/hospital.json", 
            allCollections.ToJson(
                new JsonWriterSettings {Indent = true}
            )
        );
    }
}