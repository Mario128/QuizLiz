using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Digests;
using QuizLiz.Models;
using QuizLiz.Models.db;
using QuizLiz.Models.ViewModels;
using System.Drawing;
using System.IO;
using Svg;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace QuizLiz.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Quiz()
        {
            ViewBag.Message = "Start Quiz";

            IQuestionRepository repo = new QuestionRepository();
            try
            {
                repo.Open();
                ViewBag.Topics = repo.GetAllTopics();
            }
            catch (MySqlException ex)
            {
                return View("Message", new Message("Fehler beim Abfragen der Quizdaten!", "", "Die Fragen konnten nicht ermittelt werden.", ""));
            }
            finally
            {
                repo.Close();
            }


            return View();
        }


        [HttpGet]
        public ActionResult Image()
        {
            String path = "http://commons.wikimedia.org/wiki/Special:FilePath/" + Session["pic"];
            return base.File(path, "image/svg");
        }

        [HttpGet]
        public ActionResult PictureQuiz(int id = 3)
        {
            if (Session["User"] != null)
            {
                QuestionRepository qr = new QuestionRepository();
                try
                {
                    qr.Open();
                    QuestionViewModel qvm = qr.CreateQuestion(id);
                    //CreateImageFromURL("http://commons.wikimedia.org/wiki/Special:FilePath/" + qvm.ImageName);


                    // Score als Session-wert erstellen und je nach Auswahl (falsch / richtig) anpassen
                    if (Session["score"] == null)
                    {
                        Session["score"] = 0;
                    }






                    string answer = Request.Params.Get("answer");

                    if (answer != null)
                    {
                        string feedback = "Falsch!!";
                        TempData["feedback"] = feedback;
                        string sol = Session["sol"] + "";


                        int score = Convert.ToInt32(Session["score"]);
                        if (answer.Equals(sol))
                        {
                            //feedback = "Richtig!!!"; macht Session result
                            score++;
                            Session["result"] = "Richtig!";
                        }
                        else
                        {
                            string picname = ((string)Session["pic"]);
                            Session["result"] = "Leider falsch! Leider war dies " + picname.Substring(0, picname.Length - 4);
                            score--;
                        }

                        Session["score"] = score;
                    }
                    else
                    {
                        Session["result"] = null;
                    }


                    Session["sol"] = qvm.Pos;
                    Session["pic"] = qvm.Solution;
                    return View(qvm);
                }
                catch (MySqlException ex)
                {
                    return View("Message", new Message("Fehler beim Abfragen der Quizdaten!", "", "Die Fragen konnten nicht ermittelt werden.", ""));
                }
                finally
                {
                    qr.Close();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

            IQuestionRepository qr;

        public ActionResult Map()
        {
           if(Session["User"] != null)
            {         
             try
                {
                    qr = new QuestionRepository();
                    qr.Open();

                    Question q = qr.GetLabel();
                
                    string country = Translate(q.Label);
                    ViewBag.Label = country;
               
                    return View();
                
                }
            catch (MySqlException ex)
            {
                throw;
            }
            finally
            {
                qr.Close();

            }
                
        }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }


        public ActionResult Result()
        {
                if(Session["correctCountries"] == null)
                    {
                         ViewBag.correctCountries = 0;
                         Session.Remove("correctCountries");
                         return View("Result");
                    }
                else
                    {

                        ViewBag.correctCountries = Convert.ToString(Session["correctCountries"]);
                        Session.Remove("correctCountries");
                        return View("Result");
                    }
        }

        public JsonResult CalculateResult(string eventCountry, string actualCountry)
        {
            
            if (actualCountry.Trim().ToLower() == eventCountry.Trim().ToLower())
            {
                if (Session["correctCountries"] == null)
                {
                    Session["correctCountries"] = 1;
                   
                    int uid = (int)Session["UserID"];
                    IncreaseScore(uid);

                    return Json("correctAnswer", JsonRequestBehavior.AllowGet);                    
                }
                else
                {
                    Session["correctCountries"] = Convert.ToInt32(Session["correctCountries"]) + 1;

                    int uid = (int)Session["UserID"];
                    IncreaseScore(uid);

                    return Json("correctAnswer", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("wrongAnswer", JsonRequestBehavior.AllowGet);
            }
        }

        public string Translate(string country)
        {
            var german = "de";
            var english = "en";
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={english}&tl={german}&dt=t&q={HttpUtility.UrlEncode(country)}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            try
            {
                var result = webClient.DownloadString(url);
                result = result.Substring(4, result.IndexOf("\"", 4) - 4);
                return result;
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        public void IncreaseScore(int userId)
        {
            IUserRepositiory rep = new UserRepositiory();
            try
            {
                rep.Open();
                rep.IncreaseHighscore(userId);
            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                rep.Close();
            }
        }

        public void CreateImageFromURL(string imgURL)
        {
            string ext = Path.GetExtension(imgURL);
            if (ext.Equals(".svg"))
            {
                var svgDocument = Svg.SvgDocument.Open(imgURL);
                svgDocument.ShapeRendering = SvgShapeRendering.Auto;

                Bitmap bmp = svgDocument.Draw(12, 12);
                bmp.Save("/Content/media/quiz/quizimage.jpg", ImageFormat.Jpeg);

            }

            else
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(imgURL);
                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        using (var yourImage = System.Drawing.Image.FromStream(mem))
                        {
                            yourImage.Save("c:/temp/quizimage.jpg", ImageFormat.Jpeg);
                        }
                    }
                }
            }
        }

    }
}