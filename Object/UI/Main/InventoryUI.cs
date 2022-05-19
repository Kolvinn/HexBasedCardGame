using Godot;
using System;

public class InventoryUI : TextureRect
{
    public VBoxContainer AllTab;

    private InventorySlot hovered;

    public enum InventoryState 
    {
        Drag,
        Drop,
        Default
    }

    public InventoryState inventoryState = InventoryState.Default;
    //public 
    public override void _Ready()
    {
        AllTab = this.GetNode<VBoxContainer>("TextureRect/HBoxContainer/TabContainer/All");
    }

    public InventorySlot AddNewEntry(Texture tex = null, int amount = 0, string resName = "")
    {
        var slot = Params.LoadScene<InventorySlot>("res://Object/UI/InventorySlot.tscn");
        AllTab.GetNode<GridContainer>("MarginContainer/GridContainer").AddChild(slot);
       // slot.resName = resName;
        slot.UpdateSlot(tex, amount, resName);
        slot.Connect(nameof(InventorySlot.MouseEntered), this, nameof(SlotHovered));
        slot.Connect(nameof(InventorySlot.MouseExited), this, nameof(SlotExited));
        return slot;
        
    }

    public void UpdateExistingEntry()
    {

    }

    public void SlotHovered(InventorySlot slot)
    {
        this.hovered = slot;
    }

    public void SlotExited(InventorySlot slot)
    {
        if(this.hovered ==slot)
            this.hovered = null;
    }

    //public InventorySlot GetHoveredSlot()

    public override object GetDragData(Vector2 position)
    {
        this.inventoryState = InventoryState.Drag;
        //do something with the hovered slot
        return null;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
