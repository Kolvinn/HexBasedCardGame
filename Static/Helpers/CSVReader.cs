
using System.Web;
using System.Collections.Generic;
using Godot;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection ;
public static class CSVReader
{
    public static CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null
    };
    public static string CardCSVJSON = "[{\"FrontImagePath\":\"res://Assets/FB002.png\",\"BackImagePath\":\"res://Assets/Sprites/Cards/0 - Back/Back-Elegant-With-Texture.png\",\"Rarity\":\"Bronze\",\"Cost\":2,\"ObjectId\":\"1\",\"Description\":\"[center]Summon a fireball and deal [color=#b31f1f]15[/color] damage to [color=#3b5c73]1[/color] hex.[/center] \",\"Name\":\"Fireball\",\"RequiredResources\":{\"ObjectId\":\"1\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}},{\"FrontImagePath\":\"res://Assets/Sprites/Shields/512X512/7.png\",\"BackImagePath\":\"res://Assets/Sprites/Cards/0 - Back/Back-Elegant-With-Texture.png\",\"Rarity\":\"Bronze\",\"Cost\":2,\"ObjectId\":\"2\",\"Description\":\"[center]Shield 1 allied unit for 15 hp[/center]\",\"Name\":\"Basic Shield\",\"RequiredResources\":{\"ObjectId\":\"2\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}},{\"FrontImagePath\":\"res://Assets/Sprites/swordSlash.png\",\"BackImagePath\":\"res://Assets/Sprites/Cards/0 - Back/Back-Elegant-With-Texture.png\",\"Rarity\":\"Bronze\",\"Cost\":2,\"ObjectId\":\"3\",\"Description\":\"[center]Swipe at 1 enemy hex and deal 15 damage[/center]\",\"Name\":\"Swipe\",\"RequiredResources\":{\"ObjectId\":\"3\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}},{\"FrontImagePath\":\"res://Documents/pngfind.com-dirt-splatter-png-1250135.png\",\"BackImagePath\":\"res://Assets/Sprites/Cards/0 - Back/Back-Elegant-With-Texture.png\",\"Rarity\":\"Bronze\",\"Cost\":1,\"ObjectId\":\"4\",\"Description\":\"[center]Muddify a single tile [/center]\",\"Name\":\"Mudify\",\"RequiredResources\":{\"ObjectId\":\"4\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}}]";
    public static string BuildingsCSVJSON = "[{\"TextureResource\":\"res://Assets/Sprites/20.09 - Traveler's Camp 1.1/BedRoll.tscn\",\"Type\":\"Housing\",\"ObjectId\":\"b1\",\"Description\":\"A rudimentary sleeping roll constructed out of big leaves\",\"Name\":\"Leaf-Sleep-Roll\",\"RequiredResources\":{\"ObjectId\":\"b1\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}},{\"TextureResource\":\"res://Assets/Sprites/20.09 - Traveler's Camp 1.1/Tent.tscn\",\"Type\":\"Housing\",\"ObjectId\":\"b2\",\"Description\":\"A small tent fashioned out of leaves and wood.\",\"Name\":\"Basic Tent\",\"RequiredResources\":{\"ObjectId\":\"b2\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}},{\"TextureResource\":\"res://Assets/Sprites/20.09 - Traveler's Camp 1.1/BasicCamp.tscn\",\"Type\":\"Housing\",\"ObjectId\":\"b3\",\"Description\":\"A basic camp for rest and recouperation\",\"Name\":\"Fire\",\"RequiredResources\":{\"ObjectId\":\"b3\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}},{\"TextureResource\":\"res://Assets/Sprites/Icons/SignPostIcon.tscn\",\"Type\":\"Resource\",\"ObjectId\":\"r1\",\"Description\":\"Gathering post\",\"Name\":\"Gathering Post\",\"RequiredResources\":{\"ObjectId\":\"r1\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}}]";

    public static string ResourceCostsCSVJSON = "[{\"ObjectId\":\"b1\",\"Wood\":0,\"Stone\":0,\"Essence\":0,\"Leaves\":10,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0},{\"ObjectId\":\"b2\",\"Wood\":10,\"Stone\":0,\"Essence\":0,\"Leaves\":20,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0},{\"ObjectId\":\"b3\",\"Wood\":10,\"Stone\":10,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0},{\"ObjectId\":\"r1\",\"Wood\":1,\"Stone\":0,\"Essence\":0,\"Leaves\":0,\"Leaf_Bed_Roll\":0,\"Basic_Tent\":0}]";
   


    public static List<CardModel> LoadCardCSV(){
        
        //string path = "D:/Games/GoDot Games/HexBasedCardGame/Documents/CardModels.csv";
        string path = "./Documeasedfasdfasdfnts/CardModels.csv";
        List<CardModel> models = new List<CardModel>();
        List<CardModel> modelList = new List<CardModel> ();

        try{
            using var streamReader = System.IO.File.OpenText(path);
            using var csvReader = new CsvReader(streamReader, csvConfig);
            var cardModels = csvReader.GetRecords<CardModel>();
            // foreach (object o in csvReader.GetRe)
            foreach(var card  in cardModels)
            {

                Params.Print("{0} {1} {2} {3}",card.ObjectId,card.Name,card.Rarity,card.Cost);
                modelList.Add(card);
            
            }

        }
        catch(System.Exception e)
        {
            GD.Print("Found exception, getting stored data instead");
            modelList = JsonConvert.DeserializeObject<List<CardModel>>(CardCSVJSON);
        }

        return modelList;

        //ShouldSkipRecord
    }   

    public static void PrintStringReplacement(object o)
    {
        string s = JsonConvert.SerializeObject(o);
        string newString  = "";
         //s.Where(vr => vr == '"');
        string ss = "\\" + char.ConvertFromUtf32(34);
        
        s =  s.ReplaceN('"' + "", ss);
        GD.Print(s);
    }

    public static List<BuildingModel> LoadBuildingCSV(){

        //var o = new ResourceCost();
        string path = "./Documents/BuildingsCSVv2.csv";
       // List<BuildingModel> models = new List<BuildingModel>();
        GD.Print("loading buildings csv");

        List<BuildingModel> modelList = new List<BuildingModel> ();

        try {
            using var streamReader = System.IO.File.OpenText(path);
            using var csvReader = new CsvReader(streamReader, csvConfig);
            //string value;

            var models = csvReader.GetRecords<BuildingModel>();
            

            foreach(var building  in models)
            {
                modelList.Add(building);
                
            }
        }
        catch(System.Exception e)
        {
            GD.Print("Found exception, getting stored data instead for BUIldingsCSV");
            GD.Print(e);
            modelList = JsonConvert.DeserializeObject<List<BuildingModel>>(BuildingsCSVJSON);
        }

       return modelList;

        //ShouldSkipRecord
    }

    public static void LoadResourceCosts(List<AbstractObjectModel> models)
    {
       // var o = new ResourceCost();
        string path = "./Documents/ResourceCostsv2csv.csv";
        //List<BuildingModel> models = new List<BuildingModel>();
        GD.Print("loading resources");

        List<ResourceCost> costList = new List<ResourceCost>();
        try {

        
            using var streamReader = System.IO.File.OpenText(path);
            using (var csvReader = new CsvReader(streamReader, csvConfig)){
                //string value;
                var resourceCosts = csvReader.GetRecords<ResourceCost>().ToList();
            
                ////GD.Print("resource costs count: ", resourceCosts);
                foreach(var cost  in resourceCosts)
                {
                    costList.Add(cost);

                }
            }
        }
        catch(System.Exception e)
        {
            GD.Print("Found exception, getting stored data instead for Resource Costs");
            costList = JsonConvert.DeserializeObject<List<ResourceCost>>(ResourceCostsCSVJSON);
        }
        
        foreach(AbstractObjectModel model in models)
        {
            var item = costList.FirstOrDefault(res => res.ObjectId == model.ObjectId);

            if(item!=null)
            {
                GD.Print("Found the right res cost for object id ", model.ObjectId);
                
                model.RequiredResources = item;
                model.RequiredResources.ResourceCostList =  model.RequiredResources.GetResourceCosts();

            }
        }



    }
}