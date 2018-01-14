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
            enUSResource.GetFactMessage = "Here's your science fact: ";
            enUSResource.HelpMessage = "You can say tell me a science fact, or, you can say exit... What can I help you with?";
            enUSResource.HelpReprompt = "You can say tell me a science fact to start";
            enUSResource.StopMessage = "Goodbye! See you again. Tạm biệt Quỳnh Anh";
            enUSResource.Facts.Add("A year on Mercury is just 88 days long.");
            enUSResource.Facts.Add("Despite being farther from the Sun, Venus experiences higher temperatures than Mercury.");
            enUSResource.Facts.Add("Venus rotates counter-clockwise, possibly because of a collision in the past with an asteroid.");
            enUSResource.Facts.Add("On Mars, the Sun appears about half the size as it does on Earth.");
            enUSResource.Facts.Add("Earth is the only planet not named after a god.");
            enUSResource.Facts.Add("Jupiter has the shortest day of all the planets.");
            enUSResource.Facts.Add("The Milky Way galaxy will collide with the Andromeda Galaxy in about 5 billion years.");
            enUSResource.Facts.Add("The Sun contains 99.86% of the mass in the Solar System.");
            enUSResource.Facts.Add("The Sun is an almost perfect sphere.");
            enUSResource.Facts.Add("A total solar eclipse can happen once every 1 to 2 years. This makes them a rare event.");
            enUSResource.Facts.Add("Saturn radiates two and a half times more energy into space than it receives from the sun.");
            enUSResource.Facts.Add("The temperature inside the Sun can reach 15 million degrees Celsius.");
            enUSResource.Facts.Add("The Moon is moving approximately 3.8 cm away from our planet every year.");

            resources.Add(enUSResource);
            return resources;
        }


        public List<string> GetStoryName()
        {
            List<string> storyName = new List<string>();
            storyName.Add("Well come to Vietname's stories. Would you want to hear story?");
            storyName.Add("One, The legend of Son Tinh and Thuy Tinh.");
            storyName.Add("Two, The Golden Star Fruit Tree.");
            storyName.Add("Three, The Saint Giong.");
            storyName.Add("Four, The Legendary Origins of the Viet People.");
            storyName.Add("Five, Legend of the Water Melon.");
            storyName.Add("Six, The Golden Star Fruit Tree.");
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

            //log.LogLine($"Skill Request Object input:");
            //log.LogLine(JsonConvert.SerializeObject(input));

            //log.LogLine($"Skill Request Object context:");
            //log.LogLine(JsonConvert.SerializeObject(context));

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
                        //log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        //log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        //log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                        break;
                    case "GetFactIntent":
                        //log.LogLine($"GetFactIntent sent: send new fact");                        
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetStoryFromResources("The Legendary Origins of the Viet People", log);                            
                        break;
                    case "GetNewFactIntent":
                        //log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetStoryFromResources("The Saint Giong", log);
                        break;
                    default:
                        //log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
                        break;
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            //log.LogLine($"Skill Response Object...");
            //log.LogLine(JsonConvert.SerializeObject(response));
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
