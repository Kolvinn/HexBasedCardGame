using System;
public class ActionWrapper : Godot.Object
{
    Action action;

    public ActionWrapper(Action action)
    {
        this.action = action;
    }
    public void Call()
    {
        action();
    }
}