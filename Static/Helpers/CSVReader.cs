
using System.Web;
using System.Collections.Generic;
using Godot;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Linq;
public static class CSVReader
{
    public static List<CardModel> LoadCardCSV(){
        string path = "D:/Games/GoDot Games/HexBasedCardGame/Documents/CardModels.csv";
        List<CardModel> models = new List<CardModel>();
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true
        };
        using var streamReader = System.IO.File.OpenText(path);
        using var csvReader = new CsvReader(streamReader, csvConfig);
        //string value;

        var cardModels = csvReader.GetRecords<CardModel>();
        List<CardModel> modelList = new List<CardModel> ();

       // foreach (object o in csvReader.GetRe)
        foreach(var card  in cardModels)
        {
            //string s = JsonConvert.SerializeObject(card);
            //models.Add(card);
            ////GD.Print(JsonConvert.DeserializeObject(s));
            ////GD.Print( "     ",card);
            Params.Print("{0} {1} {2} {3}",card.ObjectId,card.Name,card.Rarity,card.Cost);
            modelList.Add(card);
            
        }

    //    string s =  JsonConvert.SerializeObject(models);
    //    //GD.Print(s);
    //    List<CardModel> cards = JsonConvert.DeserializeObject<List<CardModel>>(s);
    //     //GD.Print(cards.Count);
       return modelList;

        //ShouldSkipRecord
    }   


    public static List<BuildingModel> LoadBuildingCSV(){

        //var o = new ResourceCost();
        string path = "D:/Games/GoDot Games/HexBasedCardGame/Documents/BuildingsCSV.csv";
       // List<BuildingModel> models = new List<BuildingModel>();
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null
        };
        using var streamReader = System.IO.File.OpenText(path);
        using var csvReader = new CsvReader(streamReader, csvConfig);
        //string value;

        var models = csvReader.GetRecords<BuildingModel>();
        List<BuildingModel> modelList = new List<BuildingModel> ();

        foreach(var building  in models)
        {
            //string s = JsonConvert.SerializeObject(card);
            //models.Add(card);
            ////GD.Print(JsonConvert.DeserializeObject(s));
            //GD.Print( "     ",building);
            //Params.Print("{0} {1} {2} {3}",building.ObjectId,building.Name,building.TextureResource,building.ResourceCosts);
            modelList.Add(building);
            
        }

    //    string s =  JsonConvert.SerializeObject(models);
    //    //GD.Print(s);
    //    List<CardModel> cards = JsonConvert.DeserializeObject<List<CardModel>>(s);
    //     //GD.Print(cards.Count);
       return modelList;

        //ShouldSkipRecord
    }

    public static void LoadResourceCosts(List<AbstractObjectModel> models)
    {
       // var o = new ResourceCost();
        string path = "D:/Games/GoDot Games/HexBasedCardGame/Documents/ResourceCosts.csv";
        //List<BuildingModel> models = new List<BuildingModel>();
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null

        };

        List<ResourceCost> costList = new List<ResourceCost>();
        using var streamReader = System.IO.File.OpenText(path);
        using (var csvReader = new CsvReader(streamReader, csvConfig)){
            //string value;
            var resourceCosts = csvReader.GetRecords<ResourceCost>().ToList();
           
            //GD.Print("resource costs count: ", resourceCosts);
            foreach(var cost  in resourceCosts)
            {
                costList.Add(cost);

            }
        }

        foreach(AbstractObjectModel model in models)
        {
            var item = costList.FirstOrDefault(res => res.ObjectId == model.ObjectId);

            if(item!=null)
            {
                //GD.Print("Found the right res cost for object id ", model.ObjectId);
                model.RequiredResources = item;
            }
        }



    }
}