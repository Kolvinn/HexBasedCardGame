using Godot;
using System;

public class PlayerUpdateEntry : VBoxContainer
{
    public TextureRect Icon;
    public Label AmountLabel;
    public Label  NameLabel;

    public int amount;
    public override void _Ready()
    {
        
        Icon = this.GetNode<TextureRect>("HBoxContainer/TextureRect");
        GD.Print("Icon: ", Icon);
        AmountLabel = Icon.GetNode<Label>("Number");
        NameLabel = this.GetNode<Label>("HBoxContainer/Label");

    }

    public void UpdateAmount(int num)
    {
        this.amount +=num;
        this.AmountLabel.Text = amount+"";
    }

}
