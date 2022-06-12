using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class DoctorSurvey : Survey
{
    public List<DoctorSurveyAnswer> Answers { get; set; }

    public DoctorSurvey(List<string> questions, List<string> ratingQuestions, string title)
        : base(questions, ratingQuestions, title)
    {
        Answers = new();
    }

    public override bool WasAnsweredBy(Person person)
    {
        return
            (from ans in Answers
            where ans.AnsweredBy == person.Id
            select ans).Any();
    }

    public override void AddAnswer(SurveyAnswer answer)
    {
        if (!(answer is DoctorSurveyAnswer))
        {
            throw new InvalidSurveyException("A doctor survey can only take doctor answers.");
        }
        var doctorAnswer = (DoctorSurveyAnswer) answer;
        doctorAnswer.Validate(this);
        Answers.Add(doctorAnswer);
    }

    public HashSet<ObjectId> AnsweredFor(Patient pat)
    {
        return
            (from ans in Answers
            where ans.AnsweredBy == pat.Id
            select ((DoctorSurveyAnswer) ans).DoctorId).ToHashSet();
    }

    public IEnumerable<(ObjectId, double?, int)> GetBestDoctors(int count)
    {
        return
            (from ans in Answers
            group ans by ((DoctorSurveyAnswer) ans).DoctorId into grp
            orderby grp.Average(ans => ans.Ratings.Average()) descending
            select (grp.Key, grp.Average(ans => ans.Ratings.Average()), grp.Count())).Take(count);

    }

    public IEnumerable<(ObjectId, double?, int)> GetWorstDoctors(int count)
    {
        return
            (from ans in Answers
            group ans by ((DoctorSurveyAnswer) ans).DoctorId into grp
            orderby grp.Average(ans => ans.Ratings.Average()) ascending
            select (grp.Key, grp.Average(ans => ans.Ratings.Average()), grp.Count())).Take(count);
    }
}