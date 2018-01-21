using Amazon.Lambda.Core;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Newtonsoft.Json;
using Alexa.NET.Request.Type;
using System.Text;
using System.Collections.Generic;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SayHi
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            var log = context.Logger;
            log.LogLine($"Skill Request Object...");
            log.LogLine(JsonConvert.SerializeObject(input));

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                innerResponse = new PlainTextOutputSpeech();               

                (innerResponse as PlainTextOutputSpeech).Text = CommonMessage.ResourceManager.GetString("WelcomeMessage");

            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;

                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = CommonMessage.ResourceManager.GetString("StopMessage");
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = CommonMessage.ResourceManager.GetString("StopMessage");
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = CommonMessage.ResourceManager.GetString("HelpMessage");
                        break;
                    case "GetMessage":
                        log.LogLine($"GetMessage sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        if (!string.IsNullOrEmpty(intentRequest.Intent.Slots["QUESTION"].Value))
                            (innerResponse as PlainTextOutputSpeech).Text = GetAnswer(intentRequest.Intent.Slots["QUESTION"].Value);
                        else
                        {
                            (innerResponse as PlainTextOutputSpeech).Text = CommonMessage.ResourceManager.GetString("DefaultResponse");
                        }
                        break;                    
                    default:
                        log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = CommonMessage.ResourceManager.GetString("DefaultResponse");
                        break;

                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));
            return response;
        }

        private string[] _keys = new string[]
             {
                 "hello",
                 "hi",
                 "how are you",
                 "how old are you",
                 "what's your name",
                 "what is your name",
                 "where are you"
             };
        public string GetAnswer(string question)
        {            
            string sKeyResult = _keys.FirstOrDefault<string>(s => question.Contains(s));
            string answer = "";
            if (string.IsNullOrEmpty(sKeyResult))
            {
                answer = "I don't know. Can you say again or change other question";
            }
            else
            {
                switch (sKeyResult.ToLower())
                {
                    case "how are you?":
                    case "hi":
                    case "hello":
                        List<string> answers = new List<string>();
                        answers.Add("I am great. How is going today?");
                        answers.Add("I am good. How's about you?");
                        answers.Add("I am fine. Thank you, How are you?");
                        answers.Add("I am tierd. How are you?");
                        answers.Add("I am not feeling well. And you?");
                        answer = "";
                        break;
                    case "how old are you":
                        answer = string.Format("I am five years old. {0}", sKeyResult);
                        break;
                    case "what's your name":
                        answer = string.Format("My name Ana. {0}", sKeyResult); ;
                        break;
                    case "where are you":
                        answer = string.Format("I am here. I stay near you. {0}", sKeyResult); ;
                        break;
                    default:
                        answer = GetRandomQuestion();
                        break;

                }
            }
            return answer;
        }
                
        private string GetRandomQuestion()
        {
            //List<string> questions = new List<string>();

            string question = "I can help you?";
            return question;
        }
    }
}
