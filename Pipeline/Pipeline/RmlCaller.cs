using Iib.RegexMarkupLanguage;
using Pipeline.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using static SharedModel.FlightLevelIntent;

namespace Pipeline
{
    class RmlCaller
    {
        private Rml rmlRegex;
        public RmlCaller()
        {
            var root = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\");

            var rmlDefinition = Path.Combine(root, @"RML\atc.rml");
            ConfigurationManager.AppSettings["dataDir"] = Path.Combine(root, @"data\rml\"); ;
            ConfigurationManager.AppSettings["dllDir"] = Path.Combine(root, @"RML\ExternalCallDll\bin\Debug\net5.0\");

            IEnumerable<CompilerException> errors;
            rmlRegex = new Rml(rmlDefinition, out errors);

            if (errors.Count() > 0)
            {
                throw new ApplicationException("RML Compile Error");
            }

        }

        public MessageContext Call(string input)
        {
            XmlDocument rmlResult = null;
            try
            {
                rmlResult = rmlRegex.Execute(input);
            }
            catch (Exception e)
            {
                // RML throws an exception if input doesn't match
            }

            if (rmlResult != null)
            {
                return ConvertToMessageContext(rmlResult, input);
            }

            return null;
        }

        private MessageContext ConvertToMessageContext(XmlDocument rmlDocument, string message)
        {

            var callSignNode = rmlDocument.SelectSingleNode("//CallSign");
            var intentsNode = rmlDocument.SelectSingleNode("//Intents");

            return new MessageContext
            {
                Message = message,
                CallSign = ExtractCallSign(callSignNode),
                Intents = ExtractIntents(intentsNode),
            };
        }

        private CallSign ExtractCallSign(XmlNode callSignNode)
        {
            if (callSignNode == null)
            {
                return null;
            }

            var airlineNode = callSignNode.SelectSingleNode("//Airline");
            var flightNumberNode = callSignNode.SelectSingleNode("//FlightNumber");

            if (airlineNode == null && flightNumberNode == null) // only if both are null!
            {
                return null;
            }

            return new CallSign
            {
                Airline = airlineNode.InnerText,
                FlightNumber = flightNumberNode.InnerText,
            };
        }

        private Dictionary<IntentType, MessageIntent> ExtractIntents(XmlNode intentsNode)
        {
            if (intentsNode == null)
            {
                return null;
            }

            var flightLevelNode = intentsNode.SelectSingleNode("//FlightLevel");
            var turnNode = intentsNode.SelectSingleNode("//Turn");
            var contactNode = intentsNode.SelectSingleNode("//Contact");
            var squawkNode = intentsNode.SelectSingleNode("//Squawk");

            var intents = new Dictionary<IntentType, MessageIntent>();

            if(flightLevelNode != null)
            {
                intents.Add(IntentType.FlightLevel, ExtractIntentFlightLevel(flightLevelNode));
            }
            if (turnNode != null)
            {
                intents.Add(IntentType.Turn, ExtractIntentTurn(turnNode));
            }
            if (contactNode != null)
            {
                intents.Add(IntentType.Contact, ExtractIntentContact(contactNode));
            }
            if (squawkNode != null)
            {
                intents.Add(IntentType.Squawk, ExtractIntentSquawk(squawkNode));
            }

            return intents;
        }

        private FlightLevelIntent ExtractIntentFlightLevel(XmlNode flightLevelNode)
        {
            var instructionNode = flightLevelNode.SelectSingleNode("//Instruction");
            var levelNumberNode = flightLevelNode.SelectSingleNode("//LevelNumber");

            if (instructionNode == null && levelNumberNode == null) // only if both are null!
            {
                return null;
            }

            return new FlightLevelIntent
            {
                Score = 1,
                Instruction = StringToFlightlevelInstruction(instructionNode?.InnerText),
                Level = levelNumberNode?.InnerText,
            };
        }

        private FlightLevelInstruction? StringToFlightlevelInstruction(string instruction)
        {
            switch (instruction)
            {
                case "descend":
                case "descent":
                    return FlightLevelInstruction.Descend;
                    break;
                case "climb":
                    return FlightLevelInstruction.Climb;
                    break;
                case "maintain":
                    return FlightLevelInstruction.Maintain;
            }
#if DEBUG
            if(instruction != null)
            {
                Console.WriteLine($"[DEBUG] Could not recognize FlightLevelInstruction {instruction}");
            }
#endif

            return null;
        }

        private TurnIntent ExtractIntentTurn(XmlNode turnNode)
        {
            var directionNode = turnNode.SelectSingleNode("//Direction");
            var headingNumberNode = turnNode.SelectSingleNode("//HeadingNumber");
            var degreesNode = turnNode.SelectSingleNode("//Degrees");
            var placeNode = turnNode.SelectSingleNode("//Place");

            if (directionNode == null && headingNumberNode == null
                && degreesNode == null && placeNode == null) // only if all are null
            {
                return null;
            }

            return new TurnIntent
            {
                Score = 1,
                Direction = directionNode?.InnerText,
                Heading = headingNumberNode?.InnerText,
                Degrees = degreesNode?.InnerText,
                Place = placeNode?.InnerText,
            };
        }

        private ContactIntent ExtractIntentContact(XmlNode contactNode)
        {
            var nameNode = contactNode.SelectSingleNode("//Name");
            var frequencyNode = contactNode.SelectSingleNode("//Frequency");

            if (nameNode == null && frequencyNode == null) // only if all are null!
            {
                return null;
            }

            return new ContactIntent
            {
                Score = 1,
                Place = nameNode?.InnerText,
                Frequency = frequencyNode?.InnerText,
            };
        }

        private SquawkIntent ExtractIntentSquawk(XmlNode squawkNode)
        {
            var codeNode = squawkNode.SelectSingleNode("//Code");

            if (codeNode == null)
            {
                return null;
            }

            return new SquawkIntent
            {
                Score = 1,
                Code = codeNode?.InnerText,
            };
        }


    }
}
