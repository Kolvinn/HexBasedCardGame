using System;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
public class CardModel : AbstractObjectModel {

    public string FrontImagePath {get;set;}
    public string BackImagePath {get;set;}

    public State.CardRarity Rarity {get;set;}

    public int Cost{get;set;}
    public CardModel(){

    }




}