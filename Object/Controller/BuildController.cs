
using Godot;
using System;
using System.Linq;
public class BuildController: Node
{   [Signal]
    public delegate void BuildCancel(BuildController controller);
    public TextureRect buildTexture;
    public Node2D buildSprite;
    public HexGrid grid;
    public CanvasLayer canvasLayer;
    public Player player;
    private Vector2 last;

    public BuildingModel model;
    bool something = true;
    public YSort environmentLayer;

    public PlayerController playerController;

    public State.BuildState buildState = State.BuildState.Default;

    public Building ConstructedBuilding;

    public override void _Ready()
    {

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
                HexGrid.hoveredHex.staticHitBox.Visible = true;
                HexGrid.hoveredHex.staticHitBox.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Polygon =  HexGrid.hoveredHex.topPoly.Polygon;
                // HexGrid.hoveredHex.building  = new Building();
                Building b = new Building();
               
                if(model.Name == "Fire")
                {
                    b.buildingAsset = Params.LoadScene<AnimatedSprite>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/AnimatedFire.tscn");
                    SetNodeParams(b.buildingAsset);
                }
                else
                    b.buildingAsset  = buildSprite;
                

                b.model = this.model;
                ConstructedBuilding = b;
                HexGrid.hoveredHex.building = b;
                this.environmentLayer.AddChild(b.buildingAsset);
                something = false;

                foreach(var item in this.model?.RequiredResources.GetResourceCosts()){
                    this.playerController.ResourceUpdate[item.Key] -= item.Value;
                }
                //GetViewport().SetInputAsHandled();
                this.buildState = State.BuildState.BuildFinish;
                GameController.gameState  = State.GameState.Continue;

            }
        }
    }
    public override void _Process(float delta)
    {
       

        

        // Vector2 globalPos = this.player.GetNode<Camera2D>("Camera2D").GlobalPosition;
        
        // Vector2 cameraGlobalOrigin = globalPos - new Vector2(980,540);

        // if(cameraGlobalOrigin.x<= this.player.GetNode<Camera2D>("Camera2D").LimitLeft)
        //     cameraGlobalOrigin.x = this.player.GetNode<Camera2D>("Camera2D").LimitLeft;

        // if(cameraGlobalOrigin.y<= this.player.GetNode<Camera2D>("Camera2D").LimitTop)
        //     cameraGlobalOrigin.y = this.player.GetNode<Camera2D>("Camera2D").LimitTop;


        // //this.buildSprite.GlobalPosition = GetViewport().GetMousePosition();
        // if(last != cameraGlobalOrigin){
        //     //GD.Print("camera global origin : ",cameraGlobalOrigin);
        //     // //GD.Print("Mouse Position: ", GetViewport().GetMousePosition());
        //     // //GD.Print("player global Position: ",this.player.GlobalPosition);
        //     // //GD.Print("Sprite global pos: ", this.buildSprite.GlobalPosition);
        // }

        // ////GD.Print(this.player.GetNode<Camera2D>("Camera2D").GlobalPosition);
        // last = cameraGlobalOrigin;
        // this.buildSprite.GlobalPosition = new Vector2( last.x + GetViewport().GetMousePosition().x, last.y + GetViewport().GetMousePosition().y);
        // ////GD.Print(this.buildSprite.Position);
    }
}