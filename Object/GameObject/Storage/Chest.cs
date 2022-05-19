using Godot;
public class Chest : StaticInteractable
{
    public override void _Ready()
    {
        base._Ready();
        CloseChest();
    }
    public void CloseChest()
    {
        
        this.GetNode<Sprite>("Close").Visible = true;
        this.GetNode<Sprite>("Open").Visible = false;
    }

    public void OpenChest()
    {
        this.GetNode<Sprite>("Close").Visible = false;
        this.GetNode<Sprite>("Open").Visible = true;
    }

    public bool IsOpen()
    {
        return this.GetNode<Sprite>("Open").Visible;
    }

}