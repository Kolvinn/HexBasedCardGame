using Godot;
using System;

public class GameController : Node
{   
    private CardController cardController;
    
    public GameController(){
        PopulateCardStates();
    }
    
    public override void _Ready()
    {
        
    }

    /// <summary>
    /// Loads Cards from save and sets starting state
    /// </summary>
    private void PopulateCardStates(){
        cardController = this.GetChild<CardController>(0);
    }
    


}
