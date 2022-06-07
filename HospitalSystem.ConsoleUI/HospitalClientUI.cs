using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public abstract class HospitalClientUI : ConsoleUI
{
    protected Hospital _hospital;

    protected HospitalClientUI(Hospital hospital)
    {
        _hospital = hospital;
    }
}