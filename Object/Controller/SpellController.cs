using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
public class SpellController : ControllerInstance
{   
    public Dictionary<SpellSlot,Card> spells; 
    public Node2D spellSprite;

    public Player player;

    public bool isActivePressed = false;

    public bool PerformingSpell = false;

    public YSort environmentLayer;
    public Card ActiveSpell;

    public HexHorizontalTest SpellHex;
    public SpellController()
    {

    }

    public void SetNodeParams(Node2D node){
        node.Scale = spellSprite.Scale;
        node.Position = spellSprite.Position;
        node.ZIndex = 0;
    }

    public override void _PhysicsProcess(float delta)
    {
        if(PerformingSpell)
        {

        }
        else
        {
            string action = "";
            if(!isActivePressed){
                // /string action = "";
                if(Input.IsActionJustReleased("1"))
                {
                    action = "1";
                }
                else if(Input.IsActionJustReleased("2"))
                    action = "2";

                if(!string.IsNullOrEmpty(action)){
                
                

                    var slot =spells.FirstOrDefault( s=> s.Key.BoundAction == action).Key;

                    

                    Card c = slot.BoundCard;
                    if(c!=null){
                        GD.Print("Found card :", c.model.Name , " and slot: ", slot);
                        ActiveSpell = c;
                        FetchSpell(c.model.Name);
                        slot.Border.Visible = true;  
                        slot.Update();
                        slot.Border.Update();
                        isActivePressed = true;
                    } 
                }
            }
            else
            {

                if(Input.IsActionJustPressed("right_click"))
                {
                    this.RemoveChild(spellSprite);
                    spellSprite.QueueFree();
                    isActivePressed = false;
                    spellSprite = null;
                }
                
                if(HexGrid.hoveredHex !=null && spellSprite!= null){
                    spellSprite.Position = HexGrid.hoveredHex.Position;

                    if(Input.IsActionJustPressed("left_click"))
                    {
                        CreateSpell();
                        isActivePressed = false;
                    }
                }
            }
        }
        
        
    }

    public void CreateSpell()
    {
        if(ActiveSpell.model.Name == "Fireball")
        {
            spellSprite.GetParent().RemoveChild(spellSprite);
            GD.Print("create spell for fireball: " +this.environmentLayer+ "   "+spellSprite);
            //this.environmentLayer.AddChild(spellSprite);
            SpellHex = HexGrid.hoveredHex;
            InteractWithHex(SpellHex);


        }
    }
    private Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 q0 = p0.LinearInterpolate(p1, t);
        Vector2 q1 = p1.LinearInterpolate(p2, t);
        Vector2 r = q0.LinearInterpolate(q1, t);
        return r;
    }   

    public void InteractWithHex(HexHorizontalTest hovered)
    {
        if(hovered != null)
        {
            if(hovered.building != null)
            {
                if(hovered.building.model.Name == "Fire")
                {
                    GD.Print("making fire go brrrr with spellSprite: ", spellSprite.Position);
                    spellSprite.Position = player.Position;
                    this.environmentLayer.AddChild(spellSprite);
                    
                    Tween t = new Tween();
                    t.Connect("tween_all_completed", this, nameof(CompletedAnimation));
                    t.InterpolateProperty(spellSprite, "position", spellSprite.Position, hovered.Position,1f,Tween.TransitionType.Quart);
                    this.PerformingSpell = true;
                    this.environmentLayer.AddChild(t);
                    t.Start();
                    // Node2D newNode = Params.LoadScene<AnimatedSprite>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/AnimatedFire.tscn");
                    // Params.CloneWorldParams(baseNode,newNode);
                    // hovered.building.buildingAsset.GetParent().RemoveChild(baseNode);
                    // hovered.building.buildingAsset = newNode;
                    // this.environmentLayer.AddChild(newNode);
                    // baseNode.QueueFree();
                    
                }
            }
            else if(!hovered.isBasicResource && hovered.HexEnv != null)
            {
                FireController fire = new FireController();
                fire.environmentLayer = this.environmentLayer;
                //fire.fireTile = hovered;
                this.AddChild(fire);
                fire.StartNewFire(hovered);
            }
        }
    }


    public void CompletedAnimation()
    {
        Node2D baseNode = SpellHex.building.buildingAsset;
        Node2D newNode = Params.LoadScene<AnimatedSprite>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/AnimatedFire.tscn");
        Params.CloneWorldParams(baseNode,newNode);
        SpellHex.building.buildingAsset.GetParent().RemoveChild(baseNode);
        SpellHex.building.buildingAsset = newNode;
        this.environmentLayer.AddChild(newNode);
        spellSprite.QueueFree();
        this.PerformingSpell = false;
        baseNode.QueueFree();      
    }
    public void FetchSpell(string spell)
    {
        if(spell == "Fireball")
        {
            spellSprite = Params.LoadScene<AnimatedSprite>("res://Assets/Particles/firespritesFX_PIPO/Fire.tscn");
            this.AddChild(spellSprite);
            spellSprite.Scale = new Vector2(2,2);
            spellSprite.ZIndex =5;
        }
    }





}