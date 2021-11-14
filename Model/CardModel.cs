public class CardModel : AbstractObjectModel {

    
    public CardModel(){

    }

    public string FrontImagePath {get;set;}
    public string BackImagePath {get;set;}
    
    public enum Rarity{
        Bronze,
        Silver,
        Gold,
        Platnum
    }



}