using Godot;
using System;

public class HealthBar : Control
{
    public TextureProgress bar;
    public Label healthLabel;
    public override void _Ready()
    {   
        bar  = this.GetNode<TextureProgress>("NinePatchRect/MarginContainer/TextureProgress");
        healthLabel = this.GetNode<Label>("Label");
    }

    public void SetData(int maxValue, int step, int value)
    {
        bar.MaxValue = maxValue;
        bar.Step = step;
        bar.Value = value;
        UpdateHealthLabel();
        
    }

    private void UpdateHealthLabel()
    {
        healthLabel.Text = bar.Value +"/" +bar.MaxValue;
    }

    public void UpdateHealth(int amount)
    {
        GD.Print("taking ",amount, " damage");
        bar.Value += amount;
        UpdateHealthLabel();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
