namespace Hospital
{
    enum Role
    {
        DOCTOR,
        PATIENT,
        SECRETARY,
        DIRECTOR
    }
    class User
    {
        private string username;
        private string password;
        private Role role;
        private Person person;
    }
} 
