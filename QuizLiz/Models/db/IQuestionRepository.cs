using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizLiz.Models.ViewModels;

namespace QuizLiz.Models.db
{
    interface IQuestionRepository
    {

        void Open();
        void Close();

        List<Question> GetAllPictures();
        List<Question> GetPicturesByTopic(/*int tID*/);
        QuestionViewModel CreateQuestion(int topicId);
        Dictionary<int, string> GetAllTopics();

        Question GetLabel();
    }
}
