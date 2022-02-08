using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizLiz.Models
{
    public class Question
    {
        // properties
        public int QuestionID { get; set; }
        public int TopicID { get; set; }
        public string ImageName { get; set; }
        public  string Label { get; set; }
        public  string Description { get; set; }
        public  double Longitude { get; set; }
        public  double Latitude { get; set; }

        //ctor
        public Question() : this(0, 0, "", "", "", 0.0, 0.0) { }

        public Question(int qid, int topicId, string imageName, string label, string description, double lng, double lat)
        {
            this.QuestionID = qid;
            this.TopicID = topicId;
            this.ImageName = imageName;
            this.Label = label;
            this.Description = description;
            this.Longitude = lng;
            this.Latitude = lat;
        }

        


    }
}