using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizLiz.Models.ViewModels
{
    public class QuestionViewModel
    {
        // Properties
        public string ImageName { get; set; }
        public string Solution { get; set; }
        public List<string> OtherChoices { get; set; }
        public int Pos { get; set; }

        //ctor
        public QuestionViewModel() : this("", "", new List<string>()) { }

        public QuestionViewModel(string imageName, string solution, List<string> otherChoices)
        {
            this.ImageName = imageName;
            this.Solution = solution;
            this.OtherChoices = otherChoices;
            this.Pos = new Random().Next(0,4);
        }
    }
}