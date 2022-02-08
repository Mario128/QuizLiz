using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizLiz.Models;
using MySql.Data.MySqlClient;
using QuizLiz.Models.db;

namespace QuizLiz.Controllers
{
    public class UserController : Controller
    {
        IUserRepositiory usersRepository;

        [HttpGet]
        public ActionResult Logout()
        {
            Session["isAdmin"] = null;

            return RedirectToAction("login", "user");
        }

        // GET: User
        public ActionResult Index()
        {

            return View();

        }

        [HttpGet]
         public ActionResult Login()
        {
            return View(new Login());
        }
        [HttpPost]
        public ActionResult Login(Login user)
         {
            try
            { 
                usersRepository = new UserRepositiory();
                usersRepository.Open();

                UserRole log = usersRepository.Authenticate(user.UsernameOrEmail, user.Password);
                if(log == UserRole.IsLocked)
                {
                    return View("Message", new Message("Login", "Gesperrt", "Sie sind leider vom Admin gesperrt worden", "Kontaktieren Sie den Admin"));
                }
                if(log == UserRole.Administrator)
                {
                    User u = usersRepository.GetUser(user.UsernameOrEmail, user.Password);
                    Session["isAdmin"] = 0;
                    Session["UserID"] = u.ID;
                    Session["User"] = u;
                    


                    return RedirectToAction("index", "home");
                }
                else if(log == UserRole.RegisteredUser)
                {
                    User u = usersRepository.GetUser(user.UsernameOrEmail, user.Password);
                    Session["isAdmin"] = 1;
                    Session["User"] = u;
                    Session["UserID"] = u.ID;


                    return RedirectToAction("index", "home");

                }
                else if(log == UserRole.NoUser)
                {
                    Session["isAdmin"] = 2;
                    return View("Message_Registrierung", new Message("Login", "Sie sind noch kein Benutzer", "Es tut uns leid wir konnten dich in der Datenbank nicht finden :(", "Registrieren!!!")); 
                }
                else
                {
                    Session["isAdmin"] = 2;
                    return View("Message_Registrierung", new Message("Login", "", "Es ist während der Anmeldung zu einem Fehler gekommen :(", "Versuchen Sie es später erneut"));
                }
                
            }
            catch (Exception)
            {
                Session["isAdmin"] = 2;
                return View("Message", new Message("Datenbankfehler", "", "Probleme mit der Datenbankverbindung!", "Versuchen Sie es später erneut."));
            }
            finally
            {
                usersRepository.Close();
           }

        }
        [HttpGet]
        public ActionResult Registration()
        {
            User u = new Models.User();
            return View(u);
        }
        [HttpPost]
        public ActionResult Registration(User user)
        {
            if (user == null)
            {
                return View(user);
            }

            ValidateRegistrationForm(user);

            if (ModelState.IsValid)
            {
                try
                {
                    usersRepository = new UserRepositiory();
                    usersRepository.Open();

                    if (usersRepository.Insert(user))
                    {
                        return View("Message_Registrierung", new Message("Registrierung", "", "Sie wurden erfolgreich registriert!", ""));
                    }
                    else
                    {
                        return View("Message_Registrierung", new Message("Registrierung", "", "Sie konnten nicht registriert werden!", "Versuchen Sie es später erneut."));
                    }

                }
                catch (MySqlException)
                {
                    return View("Message_Registrierung", new Message("Datenbankfehler", "", "Probleme mit der Datenbankverbindung!", "Versuchen Sie es später erneut."));
                }
                finally
                {
                    usersRepository.Close();
                }
            }

            return View(user);
        }

        private void ValidateRegistrationForm(User userToValidate)
        {
            UserRepositiory ur = new UserRepositiory();

            ur.Open();
            

            if ((userToValidate.Firstname == null) || (userToValidate.Firstname.Trim().Length < 1))
            {
                ModelState.AddModelError("Firstname", "Bitte geben Sie einen sinnvollen Vornamen ein");
            }
            if ((userToValidate.Lastname == null) || (userToValidate.Lastname.Trim().Length < 1))
            {
                ModelState.AddModelError("Lastname", "Bitte geben Sie einen sinnvollen Nachnamen ein");
            }
            if ((userToValidate.Email == null) || (!userToValidate.Email.Contains("@")))
            {
                ModelState.AddModelError("Email", "Bitte geben Sie eine gültige Email an");
            }
            if (userToValidate.Birthdate >= (DateTime.Now))
            {
                ModelState.AddModelError("Birthdate", "Kommen Sie aus der Zukunft????");
            }
            if (ur.CheckDoubleUsername(userToValidate) == false)
            {
                ModelState.AddModelError("Username", "Der Benutzername ist leider schon vergeben");
            }
            if(ur.CheckDoubleEmail(userToValidate) == false)
            {
                ModelState.AddModelError("Email", "Es besteht bereits ein Konto mit dieser Email");
            }
            if (userToValidate.Username == null)
            {
                ModelState.AddModelError("Username", "Bitte geben Sie einen Benutzernamen ein.");
            }
            if ((userToValidate.Password == null) || (userToValidate.Password.Length < 8))
            {
                ModelState.AddModelError("Password", "Das Passwort muss mindestens 8 Zeichen beinhalten");
            }
            if(userToValidate.Password != userToValidate.PasswordWH)
            {
                ModelState.AddModelError("PasswordWH", "Die Passwörter stimmen nicht überein!");
            }
        }

        public ActionResult Leaderboard()
        {
            try
            {
                usersRepository = new UserRepositiory();
                usersRepository.Open();

                return View(usersRepository.GetHighestRatedUsers());
            }
            catch (MySqlException)
            {
                return View("Message", new Message("Datenbankfehler", "", 
                "Probleme mit der Datenbank.", "Versuchen Sie es später erneut."));
            }
            finally
            {
                usersRepository.Close();
            }
        }


    }
}