using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using lab3.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace lab3.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Quiz()
        {
            if (Request.Method == "POST")
            {
                string answer = Request.Form["result"];
                if (answer != "")
                {
                    List<double> answers = HttpContext.Session.GetObjectFromJson<List<double>>("answers") ?? new List<double>();
                    answers.Add(Convert.ToDouble(answer));
                    HttpContext.Session.SetObjectAsJson("answers", answers);
                }
            }
            Random random = new Random();
            double firstNumber = random.Next(0, 10);
            double secondNumber = random.Next(0, 10);
            int operationInt = random.Next(0, 4);
            string operation = "";
            double rightAnswer = 0;
            switch (operationInt)
            {
                case 0:
                    operation = "+";
                    rightAnswer = firstNumber + secondNumber;
                    break;
                case 1:
                    operation = "-";
                    rightAnswer = firstNumber - secondNumber;
                    break;
                case 2:
                    operation = "*";
                    rightAnswer = firstNumber * secondNumber;
                    break;
                case 3:
                    operation = "/";
                    rightAnswer = firstNumber / secondNumber;
                    break;
            }
            ViewData["firstNumber"] = firstNumber;
            ViewData["secondNumber"] = secondNumber;
            ViewData["operation"] = operation;

            List<double> firstNumbers = HttpContext.Session.GetObjectFromJson<List<double>>("firstNumbers") ?? new List<double>();
            firstNumbers.Add(firstNumber);
            HttpContext.Session.SetObjectAsJson("firstNumbers", firstNumbers);
            List<double> secondNumbers = HttpContext.Session.GetObjectFromJson<List<double>>("secondNumbers") ?? new List<double>();
            secondNumbers.Add(secondNumber);
            HttpContext.Session.SetObjectAsJson("secondNumbers", secondNumbers);
            List<String> operations = HttpContext.Session.GetObjectFromJson<List<String>>("operations") ?? new List<String>();
            operations.Add(operation);
            HttpContext.Session.SetObjectAsJson("operations", operations);
            List<double> rightAnswers = HttpContext.Session.GetObjectFromJson<List<double>>("rightAnswers") ?? new List<double>();
            rightAnswers.Add(rightAnswer);
            HttpContext.Session.SetObjectAsJson("rightAnswers", rightAnswers);

            return View();
        }

        public IActionResult QuizResult()
        {
            if (Request.Method == "POST") { 
                List<double> firstNumbers = HttpContext.Session.GetObjectFromJson<List<double>>("firstNumbers");
                firstNumbers.RemoveAt(firstNumbers.Count-1);
                ViewData["firstNumbers"] = firstNumbers;
                //ViewData["firstNumbers"] = HttpContext.Session.GetObjectFromJson<List<double>>("firstNumbers");
                List<double> secondNumbers = HttpContext.Session.GetObjectFromJson<List<double>>("secondNumbers");
                secondNumbers.RemoveAt(secondNumbers.Count-1);
                ViewData["secondNumbers"] = secondNumbers;
                //ViewData["secondNumbers"] = HttpContext.Session.GetObjectFromJson<List<double>>("secondNumbers");
                List<String> operations = HttpContext.Session.GetObjectFromJson<List<String>>("operations");
                operations.RemoveAt(operations.Count-1);
                ViewData["operations"] = operations;
                //ViewData["operations"] = HttpContext.Session.GetObjectFromJson<List<String>>("operations");
                List<double> answers = HttpContext.Session.GetObjectFromJson<List<double>>("answers");
                ViewData["answers"] = answers;
                //ViewData["answers"] = HttpContext.Session.GetObjectFromJson<List<double>>("answers");
                List<double> rightAnswers = HttpContext.Session.GetObjectFromJson<List<double>>("rightAnswers");
                int numberOfCorrectAnswers = 0;
                for (int i = 0; i < answers.Count - 1;i++)
                {
                    if (answers[i] == rightAnswers[i]) numberOfCorrectAnswers++; 
                }
                ViewData["numberOfCorrectAnswers"] = numberOfCorrectAnswers + 1;
                ViewData["numberOfAnswers"] = answers.Count;

                HttpContext.Session.Clear();
                //firstNumbers.Clear();
                //HttpContext.Session.SetObjectAsJson("firstNumbers", firstNumbers);
                //secondNumbers.Clear();
                //HttpContext.Session.SetObjectAsJson("secondNumbers", secondNumbers);
                //operations.Clear();
                //HttpContext.Session.SetObjectAsJson("operations", operations);
                //answers.Clear();
                //HttpContext.Session.SetObjectAsJson("answers", answers);
                //rightAnswers.Clear();
                //HttpContext.Session.SetObjectAsJson("rightAnswers", rightAnswers);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
