using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core
{
    public class DoctorSurvey
    {
        public string ServiceOpinion { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        
        [BsonConstructor]
        public DoctorSurvey(string serviceOpinion, int rating, string comment)
        {
            ServiceOpinion = serviceOpinion;
            Rating = rating;
            Comment = comment;
        }

        public DoctorSurvey(){}

        public override string ToString()
        {
            return "Service opinion: " + ServiceOpinion + "\nRating: " + Rating + "\nComment: " + Comment;
        }
    }
}