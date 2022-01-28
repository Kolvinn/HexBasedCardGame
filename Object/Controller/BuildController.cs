
using Godot;
using System;
using System.Linq;
public class BuildController: Node
{   
    [Signal]
    public delegate void BuildCancel(BuildController controller);
    [Signal]
    public delegate void BuildBuildingComplete(Building b);

    
    public Node2D buildSprite;

    public BuildingModel model;
    bool something = true;

    public State.BuildState buildState = State.BuildState.Default;

    public Building ConstructedBuilding;


    

    public override void _Ready()
    {

    }

    public BuildController()
    {

    }

    public BuildController(BuildingModel buildingModel)
    { 
        Sprite tex = new Sprite();
        tex.Texture = Params.LoadScene<TextureButton>(buildingModel.TextureResource).TextureNormal;  
        AddChild(tex);
        buildSprite= tex;
        model = buildingModel;
        buildSprite.Scale = new Vector2(3,3);
        buildSprite.ZIndex = 50;
        
    }

    public void SetNodeParams(Node2D node){
        node.Scale = buildSprite.Scale;
        node.Position = buildSprite.Position;
        node.ZIndex = 0;
    }

    public override void _PhysicsProcess(float delta)
    {
         if(something){
            if(HexGrid.hoveredHex !=null)
                buildSprite.Position = HexGrid.hoveredHex.Position;

            if(Input.GetActionStrength("right_click")>0)
            {

                EmitSignal(nameof(BuildCancel), this);
            }
            else if(Input.GetActionStrength("left_click")>0)
            {

                this.RemoveChild(buildSprite);
                buildSprite.ZIndex = 0;
                //HexGrid.hoveredHex.staticHitBox.Visible = true;
                //HexGrid.hoveredHex.staticHitBox.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Polygon =  HexGrid.hoveredHex.topPoly.Polygon;
                // HexGrid.hoveredHex.building  = new Building();
                Building b = new Building();
               
                if(model.Name == "Fire")
                {
                    b.buildingAsset = Params.LoadScene<AnimatedSprite>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/StaticFire.tscn");
                    SetNodeParams(b.buildingAsset);
                }
                else
                    b.buildingAsset  = buildSprite;
                
                
                b.model = this.model;
                ConstructedBuilding = b;
                HexGrid.hoveredHex.building = b;
                something = false;

                EmitSignal(nameof(BuildBuildingComplete), b);               

            }
        }
    }
}