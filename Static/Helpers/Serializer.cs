using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public static class Serializer{

    public class ControllerConverter : JsonConverter
    {

        private readonly Type[] _types;

        public ControllerConverter(params Type[] types)
        {   
            _types = types;
        }
        public ControllerConverter(){

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {   
            GD.Print("ooga booga");
            
            

            JToken t = JToken.FromObject(value);
            t.WriteTo(writer);
            // JToken t = JToken.FromObject(value);
            // GD.Print(t.Type);
            // if (t.Type != JTokenType.Object)
            // {
            //     t.WriteTo(writer);
            // }
            // else
            // {
            //     JObject o = (JObject)t;
                
            //     IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();

            //     o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));

            //     o.WriteTo(writer);
            // }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }


    public class TestModel : JsonConverter
    {

        private readonly Type[] _types;

        public TestModel(params Type[] types)
        {
            _types = types;
        }
        public TestModel(){

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {   
            GD.Print("ooga booga test model");
            GD.Print(value);
           // CardModel c = new CardModel();

            //JToken t = JToken.FromObject(value,serializer);

            //t.WriteTo(writer);
            //serializer.Serialize
            writer.WriteValue(value);
            //JsonConvert.SerializeObject(c, Formatting.Indented, new KeysJsonConverter(typeof(CardModel), typeof(TestModel)));
            //JsonConverter
            //writer.WriteValue(value);
            // JToken t = JToken.FromObject(value);
            // GD.Print(t.Type);
            // if (t.Type != JTokenType.Object)
            // {
            //     t.WriteTo(writer);
            // }
            // else
            // {
            //     JObject o = (JObject)t;
                
            //     IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();

            //     o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));

            //     o.WriteTo(writer);
            // }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }



    
    public static void ConvertToJson(GameController g){
        var something = g.GetType().GetFields(BindingFlags.NonPublic |  BindingFlags.Instance |BindingFlags.Public);
        
        foreach(FieldInfo f in something){
            GD.Print("Game controller field value: ",f.Name, "   ",f.GetType(),"    ", f.GetValue(g));
            if(typeof(ControllerBase).IsInstanceOfType(f.GetValue(g))){
                GD.Print("Found the node");
            }
        }

        var objType = g.GetType();
        var objClass = g.GetClass();
        
        var file = g.Filename;

       // var somethingElse  = 
        
    }

}