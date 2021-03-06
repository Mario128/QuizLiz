using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using QuizLiz.Models.ViewModels;


namespace QuizLiz.Models.db
{
    public class QuestionRepository : IQuestionRepository
    {
        private string _connectionString = "Server=localhost;Database=quizliz;Uid=root;SslMode=none";
        private MySqlConnection _connection;
        public void Open()
        {
            if (this._connection == null)
            {
                this._connection = new MySqlConnection(_connectionString);
            }

            if (this._connection.State != ConnectionState.Open)
            {
                this._connection.Open();
            }
        }

        public void Close()
        {
            if ((this._connection != null) && (this._connection.State == ConnectionState.Open))
            {
                this._connection.Close();
            }
        }

        public List<Question> GetAllPictures()
        {
            List<Question> allQuestions = new List<Question>();
            try
            {
                MySqlCommand cmdGetAllPictures = this._connection.CreateCommand();
                cmdGetAllPictures.CommandText = "SELECT image, label FROM questions ORDER BY rand() LIMIT 4;";

                using (MySqlDataReader reader = cmdGetAllPictures.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            allQuestions.Add(new Question
                            {
                                ImageName = Convert.ToString(reader["image"]),
                                Label = Convert.ToString(reader["label"])

                            });
                        }
                    }
                }

                return allQuestions.Count > 0 ? allQuestions : null;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<Question> GetPicturesByTopic(/*int tID*/)
        {
            List<Question> allQuestionsWithID = new List<Question>();
            try
            {
                MySqlCommand cmdGetAllPicturesWithID = this._connection.CreateCommand();
                cmdGetAllPicturesWithID.CommandText = "SELECT image, label FROM questions WHERE topicid = 3 ORDER BY rand() LIMIT 4;";
                
                //cmdGetAllPicturesWithID.Parameters.AddWithValue("topicID", tID);
                using (MySqlDataReader reader = cmdGetAllPicturesWithID.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            allQuestionsWithID.Add(new Question
                            {
                                ImageName = Convert.ToString(reader["image"]),
                                Label = Convert.ToString(reader["label"])
                                //TopicID = Convert.ToInt32(reader["topicid"])
                            });

                        }
                    }
                }

                return allQuestionsWithID.Count > 0 ? allQuestionsWithID : null;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public QuestionViewModel CreateQuestion(int topicId)
        {
            QuestionViewModel questionWithID;
            List<string> otherChoices = new List<string>(3);
            string imageName, solution;

            try
            {
                MySqlCommand cmdGetAllPicturesWithID = this._connection.CreateCommand();
                cmdGetAllPicturesWithID.CommandText = "SELECT image, label FROM questions WHERE topicid = @topicId ORDER BY rand() LIMIT 4;";

                cmdGetAllPicturesWithID.Parameters.AddWithValue("topicId", topicId);

                using (MySqlDataReader reader = cmdGetAllPicturesWithID.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        imageName = Convert.ToString(reader["image"]);
                        solution = Convert.ToString(reader["label"]);
                        while (reader.Read())
                        {
                           otherChoices.Add(Convert.ToString(reader["label"]));
                        }
                        questionWithID = new QuestionViewModel(imageName, solution, otherChoices);
                        return questionWithID;

                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (MySqlException e)
            {
                throw;
            }
        }

        public Question GetLabel()
        {
            try
            {
           MySqlCommand cmd = this._connection.CreateCommand();
            cmd.CommandText = "SELECT label FROM questions WHERE topicid = 3 ORDER BY rand() LIMIT 1;";

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new Question
                        {     
                            Label =  Convert.ToString(reader["label"]),
                        };
                    }
                }
                return null;
            }
            catch (MySqlException ex)
            {
                throw;
            }
        }

        public Dictionary<int, string> GetAllTopics()
        {
            Dictionary<int, string> topics = new Dictionary<int, string>();

            try
            {
                MySqlCommand cmdGetAllTopics = this._connection.CreateCommand();
                cmdGetAllTopics.CommandText = "SELECT * FROM topics;";

                using (MySqlDataReader reader = cmdGetAllTopics.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            topics.Add(Convert.ToInt32(reader["topicId"]), Convert.ToString(reader["name"]));
                        }
                    }
                }

                return topics.Count > 0 ? topics : null;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}