using System;
using Newtonsoft.Json.Converters;

public class CardModel : AbstractObjectModel {

    public TestModel testModel{
        get;set;
    }
    public string FrontImagePath {get;set;}
    public string BackImagePath {get;set;}
    public CardModel(){
        this.testModel = new TestModel();
        this.Name = "THIS IS A NAME";
    }

    
    
    public enum Rarity{
        Bronze,
        Silver,
        Gold,
        Platnum
    }



}