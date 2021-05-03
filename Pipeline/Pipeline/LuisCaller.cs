using LUIS;
using LUIS.Model;
using Pipeline.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedModel.FlightLevelIntent;

namespace Pipeline
{
    class LuisCaller
    {
        private UtteranceInterpreter interpreter;

        public LuisCaller(LuisConfig config)
        {
            interpreter = new UtteranceInterpreter(config);
        }

        public MessageContext Call(string input)
        {
            MessageContext messageContext = null;

            LuisResult luisResult = interpreter.Interpret(input);
            if(luisResult != null)
            {
                messageContext = ConvertToMessageContext(luisResult);
            }

            return messageContext;
        }

        private MessageContext ConvertToMessageContext(LuisResult luisResult)
        {
            MessageContext messageContext = null;

            if (luisResult != null)
            {

                // map luis result to unified model
                messageContext = new MessageContext() { Message = luisResult.Utterance };

                if (luisResult.CallSign != null)
                    messageContext.CallSign = new SharedModel.CallSign() { Airline = luisResult.CallSign.Airline, FlightNumber = luisResult.CallSign.FlightNumber };

                if (luisResult.IntentScores != null && luisResult.IntentScores.Count > 0)
                {
                    messageContext.Intents = new Dictionary<SharedModel.IntentType, MessageIntent>();
                    foreach (IntentType intentType in luisResult.IntentScores.Keys)
                    {
                        MessageIntent intentDetails = null;

                        // separate so an intent score can be set,
                        // no matter if luis could extract entities
                        switch (intentType)
                        {
                            case IntentType.Contact:
                                intentDetails = new ContactIntent();
                                break;
                            case IntentType.FlightLevel:
                                intentDetails = new FlightLevelIntent();
                                break;
                            case IntentType.Squawk:
                                intentDetails = new SquawkIntent();
                                break;
                            case IntentType.Turn:
                                intentDetails = new TurnIntent();
                                break;
                            case IntentType.None:
                                intentDetails = new NoneIntent();
                                break;
                        }

                        if (intentDetails != null)
                        {
                            // set extracted entity info
                            if (intentDetails is ContactIntent && luisResult.ContactInfo != null)
                            {
                                ContactIntent intent = intentDetails as ContactIntent;
                                intent.Frequency = luisResult.ContactInfo.Frequency;
                                intent.Place = luisResult.ContactInfo.Place;
                            }
                            else if (intentDetails is FlightLevelIntent && luisResult.FlightLevelInfo != null)
                            {
                                FlightLevelIntent intent = intentDetails as FlightLevelIntent;
                                intent.Level = luisResult.FlightLevelInfo.Level;
                                FlightLevelInstruction flInstruaction;
                                if (Enum.TryParse(luisResult.FlightLevelInfo.Instruction, true, out flInstruaction))
                                    intent.Instruction = flInstruaction;
                            }
                            else if (intentDetails is SquawkIntent)
                            {
                                (intentDetails as SquawkIntent).Code = luisResult.SquawkCode;
                            }
                            else if (intentDetails is TurnIntent && luisResult.TurnInfo != null)
                            {
                                TurnIntent intent = intentDetails as TurnIntent;
                                intent.Degrees = luisResult.TurnInfo.Degrees;
                                intent.Direction = luisResult.TurnInfo.Direction;
                                intent.Heading = luisResult.TurnInfo.Heading;
                                intent.Place = luisResult.TurnInfo.Place;
                            }

                            intentDetails.Score = luisResult.IntentScores[intentType];
                            messageContext.Intents.Add(intentType, intentDetails);
                        }
                    }
                }
            }

            return messageContext;
        }
    }
}
