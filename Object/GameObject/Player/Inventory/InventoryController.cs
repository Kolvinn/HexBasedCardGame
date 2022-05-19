using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class InventoryController : Node2D
{
    public InventoryUI UI;

    public List<InventorySlot> slots = new List<InventorySlot>();

    //private Dictionary<string,int> resources = new Dictionary<string, int>();

    //need to set this at some point
    public int limit = 30;

    public override void _Ready()
    {
        for( int i=0;i<limit;i++)
        {
            slots.Add(UI.AddNewEntry ());
        }
    }


    
    public override void _PhysicsProcess(float delta)
    {
        //if(UI.)
        if(this.UI.Visible)
        {
         
            if(UI.inventoryState == InventoryUI.InventoryState.Drag)
            {

            }
            else if(UI.inventoryState == InventoryUI.InventoryState.Drop)
            {

            }
        }
    }

    public void Remove(string name, int amount)
    {
        var allSlots = slots.Where(item => item.resName == name).ToList();
        while(amount >0 && allSlots.Count() >0)
        {
            var slot =  allSlots[0];
            var min = Math.Min(amount, slot.resAmount);
            GD.Print("removing ", name, " from slot with ",slot.resAmount);
            
            slot.UpdateSlot(slot.resTex,-min,name);
            //slot.resAmount -= min;
            amount -=min;
           // sl
            if(slot.resAmount== 0){
                slot.UpdateSlot(null,0,name);
                allSlots.Remove(slot);
            }
            // slots.Remove(slot);
            //Update
        }
    }

    public void AddItem(Texture tex, int amount, string resName)
    {
        var existing = slots.FirstOrDefault(i => i.resName == resName && i.resAmount + amount < 21);

       
        if(existing != null)
        {
            existing.UpdateSlot(tex, amount, resName);
        }
        else
        {
            var newSlot = slots.FirstOrDefault(i => i.resAmount == 0);
 
            newSlot.UpdateSlot(tex, amount, resName);
        }
    }

    public void HandleDrag()
    {
        //do something with the mouse things
    }

    public void HandleDrop()
    {

    }
    
    public void OnSlotDrag(int id)
    {

    }

    public bool CanStoreObject(GameObject obj)
    {
        return true;
    }
}