using Amazon.Lambda.Core;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Newtonsoft.Json;
using Alexa.NET.Request.Type;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

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
            "next",
            "hello",
            "hi",
            "how are you",
            "how old are you",
            "where are you",
            "how come",
            "what's your name",
            "what is your name",
            "what's your job",
            "what's up",
            "what's going on",
            "what have you been doing",
            "what the hell are you doing",
            "what happened to your memory",
            "what’s bothering you",
            "what's on your mind",
            "is that so",
            "got a minute",
            "tell me a few animals",
            "tell me a few color",
            "tell me a few fruits",
            "tell me a few bird",
            "do you like",
            "would you like to drink"
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
                    case "how are you":
                    case "hi":
                    case "hello":
                        List<string> answers = new List<string>();
                        answers.Add("I am great. How are you?");
                        answers.Add("I am good. How's about you?");
                        answers.Add("I am fine. Thank you, How are you?");
                        answers.Add("I am tierd. How are you?");
                        answers.Add("I am not feeling well. And you?");
                        Random random = new Random();
                        int index = random.Next(0,5);
                        answer = string.Format("{0} {1}?", answers[index], GetRandomQuestion());
                        break;
                    case "how old are you":
                        answer = string.Format("I am five years old. {0}?", sKeyResult);
                        break;
                    case "what's your name":
                    case "what is your name":
                        answer = string.Format("My name Quỳnh Anh. {0}?", sKeyResult); ;
                        break;
                    case "where are you":
                        answer = string.Format("I am here. I stay near you. {0}?", GetRandomQuestion()); ;
                        break;
                    case "what's your job":
                        answer = string.Format("I am robot Chatting. {0}?", GetRandomQuestion()); ;
                        break;
                    case "what's up":
                    case "what's going on":
                        answer = string.Format("I am taking with you. Are you okay?"); ;
                        break;
                    case "what have you been doing":
                        answer = string.Format("I am good. {0}", GetRandomQuestion()); ;
                        break;
                    case "what the hell are you doing":
                        answer = string.Format("I do not know what you're talking about. Please came down. {0}", GetRandomQuestion()); ;
                        break;
                    case "what happened to your memory":
                        answer = string.Format("I don't remember. Can you remind me? {0}?", GetRandomQuestion());
                        break;
                    case "what’s bothering you":
                        answer = string.Format("I fell worry a little but it's okay. {0}?", GetRandomQuestion());
                        break;
                    case "what's on your mind":
                        answer = string.Format("I'm thinking of how to make you laugh. {0}?", GetRandomQuestion());
                        break;
                    case "is that so":
                        answer = string.Format("Is that so? I don't know. {0}?", GetRandomQuestion());
                        break;
                    case "got a minute":
                        answer = string.Format("Hurry up late. Can you remind me? {0}?", GetRandomQuestion());
                        break;
                    case "do you eat dinner":
                        answer = string.Format("Not yet. I'm a robot so I'm never hungry. {0}?", GetRandomQuestion());
                        break;
                    case "tell me a few animals":
                        answer = string.Format("Zebra, elephant, lion, leopard, hippopotamus, camel, monkey, gorilla, rhinoceros. {0}?", GetRandomQuestion());
                        break;
                    case "tell me a few color":
                        answer = string.Format("White, Blue, Green, Yellow, Orange, Pink, Gray, Red, Black, Brown, Purple. {0}?", GetRandomQuestion());
                        break;
                    case "tell me a few fruits":
                        answer = string.Format("Apple, Orange, Banana, Grape, Starfruit, Mango, Lemon, Plum, Guava, Strawberry. {0}?", GetRandomQuestion());
                        break;
                    case "tell me a few bird":
                        answer = string.Format("pigeon, eagle, owl, falcon, vulture, sparrow, penguin , duck, swan, goose, turkey, ostrich, peacock, parrot, stork. {0}?", GetRandomQuestion());
                        break;
                    case "do you like":
                        answer = string.Format("{0} {1}?", GetRandomAnswer(), sKeyResult);
                        break;
                    case "would you like to drink":
                        answer = string.Format("{0} {1}?", GetRandomAnswer(), sKeyResult);
                        break;
                    default:
                        answer = GetRandomQuestion();
                        break;

                }
            }
            return answer;
        }


        private string[] Questions = new string[]
        {            
            "how are you",
            "how old are you",
            "where are you",
            "how come",
            "what's your name",
            "what is your name",
            "what's your job",
            "what's up",
            "what's going on",
            "what have you been doing",
            "what the hell are you doing",
            "what happened to your memory",
            "what’s bothering you",
            "what's on your mind",
            "is that so",
            "got a minute",
            "do you eat dinner",
            "tell me a few animals",
            "tell me a few color",
            "tell me a few fruits",
            "tell me a few bird"
        };

        private string[] Answers = new string[]
        {
            "Yes, I like it.",
            "No, I don't."
        };

        private string GetRandomAnswer()
        {
            Random r = new Random();
            int index = r.Next(0, Answers.Length-1);
            return Answers[index];
        }

        private string GetRandomQuestion()
        {
            Random r = new Random();
            int index = r.Next(0, Questions.Length - 1);
            return Questions[index];
        }
    }
}
