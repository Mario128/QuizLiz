using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizLiz.Models
{
    public enum Gender
    {
        male, female, notSpecified
    }

    public class User
    {
        

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Birthdate { get; set; }
        public Gender Gender { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordWH { get; set; }
        public string Email { get; set; }
        public int ID { get; set; }
        public bool IsLocked { get; set; }
        public int Highscore { get; set; }



        public User() : this("", "", DateTime.MinValue, Gender.notSpecified, "", "", "", "", 0, false, 0) { }
        public User(string firstname, string lastname, DateTime birthdate, Gender gender, string username, string password, string passwordwh, string email, int id, bool isLocked, int highscore)
        {
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Birthdate = birthdate;
            this.Gender = gender;
            this.Username = username;
            this.Password = password;
            this.PasswordWH = passwordwh;
            this.Email = email;
            this.ID = id;
            this.IsLocked = isLocked;
            this.Highscore = highscore;
            
        }



    }
}