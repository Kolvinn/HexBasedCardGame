using Godot;
using System;

public class InventorySlot : TextureRect
{
    public int resAmount =0;
    public Texture resTex;

    private Label amountLabel;
    private TextureRect texRect;

    public string resName;

    public int id
    {
        get;set;
    }

    [Signal]
    public delegate void MouseEntered(InventorySlot slot);

    [Signal]
    public delegate void MouseExited(InventorySlot slot);
    public override void _Ready()
    {
        this.amountLabel = this.GetNode<Label>("MarginContainer/TextureRect/Number");
        this.texRect = this.GetNode<TextureRect>("MarginContainer/TextureRect");

    }

    public void UpdateSlot(Texture tex, int amount, string resName)
    {
        this.resName = resName;
        this.resAmount += amount;
        this.resTex = tex;

        this.texRect.Texture = this.resTex;

        this.amountLabel.Text = this.resAmount + "";
        this.amountLabel.Visible = resAmount != 0;
        //this.texRect.Visible = resAmount != 0;
        this.HintTooltip = this.resName;
        Update();
        
 

    }

    public void _on_InventorySlot_mouse_entered()
    {
        EmitSignal(nameof(MouseEntered), this);
    }


    public void _on_InventorySlot_mouse_exited()
    {
        EmitSignal(nameof(MouseExited), this);
    }


}
