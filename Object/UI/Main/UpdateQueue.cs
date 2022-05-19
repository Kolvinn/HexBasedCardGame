using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public class UpdateQueue : Control
{
    VBoxContainer Updatebox;
    Dictionary<PlayerUpdateEntry, Timer> queue = new Dictionary<PlayerUpdateEntry, Timer>();

    Queue<PlayerUpdateEntry> labels = new Queue<PlayerUpdateEntry>();
    public override void _Ready()
    {
        this.Updatebox = this.GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
    }
    
    public void PushUpdate(TextureRect icon , int amount, string resName )
    {
        var existing = labels.FirstOrDefault(item => item.NameLabel.Text == resName);
        
        if(existing!=null)
        {
            existing.UpdateAmount(amount);
            //existing.AmountLabel.Text = amount+"";
            queue[existing].Stop();
            queue[existing].Start(3);

        }
        else
        {
            var entry = Params.LoadScene<PlayerUpdateEntry>("res://Object/UI/PlayerUpdateEntry.tscn");

            GD.Print(entry.Icon, " ", icon);
            
            //goes through _Ready()
            Updatebox.AddChild(entry);
            
            entry.UpdateAmount(amount);
            entry.Icon.Texture = icon.Texture;
           // entry.AmountLabel.Text = amount + "";
            entry.NameLabel.Text = resName;
           
            Timer timer = new Timer();
            this.AddChild(timer);

            timer.Connect("timeout", this, nameof(OnTimeout));
            queue.Add(entry,timer);
            labels.Enqueue(entry);

            //3 seconds stay in UI
            timer.Start(3);
        }
        // Label label = new Label();
        // label.Text = formattedUpdate;
        // label.Autowrap = true;
        // label.Align =Label.AlignEnum.Center;
        // label.Valign = Label.VAlign.Center;
        // label.SizeFlagsHorizontal = 3;

        
    }

    public void OnTimeout()
    {
        var label = labels.Dequeue();
        var timer = queue[label];
        Updatebox.RemoveChild(label);
        label.QueueFree();
        timer.QueueFree();
    }

}
