namespace HospitalSystem.Core.Surveys;

public class HospitalSurvey : Survey
{
    public List<SurveyAnswer> Answers { get; set; }

    public HospitalSurvey(List<string> questions, List<string> ratingQuestions, string title)
        : base(questions, ratingQuestions, title)
    {
        Answers = new();
    }

    public override void AddAnswer(SurveyAnswer answer)
    {
        answer.Validate(this);
        Answers.Add(answer);
    }

    public override bool WasAnsweredBy(Person person)
    {
        return
            (from ans in Answers
            where ans.AnsweredBy == person.Id
            select ans).Any();
    }
}