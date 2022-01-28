using Godot;
using System;

public class TimerLabel : Label
{
    Timer timer;
    public override void _Ready()
    {
        timer = this.GetChild<Timer>(0);
    }

    
    public override void _Process(float delta)
    {
        
        this.Text = "Humans Arrive In: ";
        if (timer !=null)
        {
            ////GD.Print("skjldnsfkjsndfknsjkdfnjksdnf");
            this.Text += timer.TimeLeft;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
