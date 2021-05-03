using Evaluation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Evaluation
{
    class AirPlaneInRangeJsonConverter : JsonConverter<AirplaneInRange>
    {
        public override AirplaneInRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected Start of Array");
            }
            reader.Read(); // start of array

            var planeId = reader.GetInt32();
            var planePosition = ReadProperty<AirplanePosition>(ref reader, typeof(AirplanePosition), options);

            if(reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException("Expected End of Array");
            }

            return new AirplaneInRange
            {
                Id = planeId,
                Position = planePosition,
            };
        }

        public override void Write(Utf8JsonWriter writer, AirplaneInRange value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        //// from https://github.com/dotnet/runtime/blob/81bf79fd9aa75305e55abe2f7e9ef3f60624a3a1/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/JsonValueConverterKeyValuePair.cs
        private T ReadProperty<T>(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            T k;

            //Attempt to use existing converter first before re-entering through JsonSerializer.Deserialize().
            //The default converter for objects does not parse null objects as null, so it is not used here.
            if (typeToConvert != typeof(object) && (options?.GetConverter(typeToConvert) is JsonConverter<T> keyConverter))
            {
                reader.Read(); // start of object
                k = keyConverter.Read(ref reader, typeToConvert, options);
                reader.Read(); // end of object
            }
            else
            {
                k = JsonSerializer.Deserialize<T>(ref reader, options);
            }

            return k;
        }
    }
}
