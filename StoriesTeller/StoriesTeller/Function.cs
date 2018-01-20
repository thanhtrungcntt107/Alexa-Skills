using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Newtonsoft.Json;
using Alexa.NET.Request.Type;
using System.Text;
using System.IO;
using System.Reflection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace StoriesTeller
{
    public class Function
    {
        public List<FactResource> GetResources()
        {
            List<FactResource> resources = new List<FactResource>();
            FactResource enUSResource = new FactResource("en-US");
            enUSResource.SkillName = "Stories Teller";
            enUSResource.GetFactMessage = "Here's your stories: ";
            enUSResource.HelpMessage = "You can say tell me a story fact, or, you can say exit... What can I help you with?";
            enUSResource.HelpReprompt = "You can say tell me a story fact to start";
            enUSResource.StopMessage = "Goodbye! Tạm biệt Quỳnh Anh! See you again.";            

            resources.Add(enUSResource);
            return resources;
        }


        public List<string> GetStoryName()
        {
            List<string> storyName = new List<string>();
            storyName.Add("Well come to Vietname's stories. Would you want to hear a story?");
            storyName.Add("One, The legend of Son Tinh and Thuy Tinh.");
            storyName.Add("Two, The Golden Star Fruit Tree.");
            storyName.Add("Three, The Saint Giong.");
            storyName.Add("Four, The Legendary Origins of the Viet People.");
            storyName.Add("Five, Legend of the Water Melon.");
            storyName.Add("Six, The Moon Boy.");
            return storyName;

        }

        public string GetStoryNameByOrderNumber(string orderNumber)
        {
            string storyName = "";
            switch(orderNumber.ToLower())
            {
                case "one":
                case "1":
                    storyName = "The legend of Son Tinh and Thuy Tinh";
                    break;
                case "two":
                case "2":
                    storyName = "The Golden Star Fruit Tree";
                    break;
                case "three":
                case "3":
                    storyName = "The Saint Giong";
                    break;
                case "four":
                case "4":
                    storyName = "The Legendary Origins of the Viet People";
                    break;
                case "five":
                case "5":
                    storyName = "Legend of the Water Melon";
                    break;
                case "six":
                case "6":
                    storyName = "The Moon Boy";
                    break;
            }
            return storyName;
        }
        public string GetStoryNameByName(string orderNumber)
        {
            string storyName = "";
            switch (orderNumber.ToLower())
            {
                case "the legend of son tinh and thuy tinh":
                    storyName = "The legend of Son Tinh and Thuy Tinh";
                    break;
                case "the golden star fruit tree":
                    storyName = "The Golden Star Fruit Tree";
                    break;
                case "the saint giong":
                    storyName = "The Saint Giong";
                    break;
                case "the legendary origins of the viet people":
                    storyName = "The Legendary Origins of the Viet People";
                    break;
                case "legend of the water melon":
                    storyName = "Legend of the Water Melon";
                    break;
                case "the moon boy":
                    storyName = "The Moon Boy";
                    break;
            }
            return storyName;
        }

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

            log.LogLine($"Skill Request Object input:");
            log.LogLine(JsonConvert.SerializeObject(input));

            log.LogLine($"Skill Request Object context:");
            log.LogLine(JsonConvert.SerializeObject(context));

            var allResources = GetResources();
            var resource = allResources.FirstOrDefault();
            var storyName = GetStoryName();

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                innerResponse = new PlainTextOutputSpeech();
                StringBuilder builder = new StringBuilder();
                foreach(string s in storyName)
                {
                    builder.AppendLine(s);
                }

                (innerResponse as PlainTextOutputSpeech).Text = builder.ToString();

            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;

                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                        break;
                    case "GetStoryIntent":
                        log.LogLine($"GetStoryIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        if (!string.IsNullOrEmpty(intentRequest.Intent.Slots["GetStoryByNameIntent"].Value))
                            (innerResponse as PlainTextOutputSpeech).Text = GetStoryFromResources(GetStoryNameByName(intentRequest.Intent.Slots["GetStoryByNameIntent"].Value), log);
                        else if (!string.IsNullOrEmpty(intentRequest.Intent.Slots["GetStoryByOrderNumberIntent"].Value))
                            (innerResponse as PlainTextOutputSpeech).Text = GetStoryFromResources(GetStoryNameByOrderNumber(intentRequest.Intent.Slots["GetStoryByOrderNumberIntent"].Value), log);
                        break;
                    case "GetNextStoryIntent":
                        //GetNextStoryIntent
                        log.LogLine($"GetNextStoryIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetStoryFromResources("The Saint Giong", log);
                        break;
                    case "GetRamdomStoryIntent":
                        log.LogLine($"GetRamdomStoryIntent sent: send new fact");
                        string story = "The Legendary Origins of the Viet People";
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetStoryFromResources(story, log);
                        break;
                    default:
                        log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
                        break;

                        
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));
            return response;
        }

        public string GetStoryFromResources(string storyName, ILambdaLogger logger = null)
        {
            String text = "Sorry, I can't get story.";
            try
            {
                var assembly = typeof(StoriesTeller.Function).GetTypeInfo().Assembly;
                string resourceName = string.Format(@"StoriesTeller.Stories.{0}.txt", storyName);
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        text = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                if (logger != null)
                    logger.LogLine(string.Format("The file could not be read: {0}", e.Message));
            }
            return text;
        }

        private string GetStory(string storyName, ILambdaLogger logger)
        {
            String text = "Sorry, I can't get story.";
            try
            {   // Open the text file using a stream reader.
                //string path = Path.Combine(Environment.CurrentDirectory, "Some\\Path.txt")
                string path = System.IO.Path.GetFullPath(string.Format(@"{0}\Stories\{1}.txt", Directory.GetCurrentDirectory(), storyName));
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(fileStream, Encoding.UTF8))
                {
                    // Read the stream to a string, and write the string to the console.
                    text = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                logger.LogLine(string.Format("The file could not be read: {0}", e.Message));
            }
            return text;
        }
    }
}
