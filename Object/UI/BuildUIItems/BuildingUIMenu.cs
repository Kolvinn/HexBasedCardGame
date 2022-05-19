using Godot;
using System;
using System.Collections.Generic;

public class BuildingUIMenu : Control
{
    
    public TabContainer buildTabs;
    public BuildMenu currentMenu;//, livingMenu, techMenu, magicMenu,ResourcesMenu;
    public List<BuildMenu> menus = new List<BuildMenu>();
    public override void _Ready()
    {
        buildTabs = this.GetNode<TabContainer>("TabContainer");
        
    }

    public void _on_TabContainer_tab_changed(int index)
    {
        this.currentMenu = buildTabs.GetChild<MarginContainer>(index).GetChild<BuildMenu>(0);
    }
    
    public void AddBuildMenus(params BuildMenu[] menus)
    {
        
        foreach(var menu in menus)
        {
            MarginContainer container = new MarginContainer()
            {
                MarginLeft = 50,
                MarginRight = 50,
                Name = menu.Name
            };
            container.AddChild(menu);
            this.buildTabs.AddChild(container);
            
            this.menus.Add(menu);
            foreach(var icon in menu.icons)
            {
                this.AddChild(icon.toolTip);
                icon.toolTip.Visible = false;
                //icon.RectPosition = icon.RectPosition - new Vector2(0,100);
            }
            //MarginContainer container = new MarginContainer(){
               // Name = menu.Name,
               // CustomMultiplayer
            //}.custom
        }
    }

    public void ShowMenu()
    {
        this.Visible = true;
        this.GetNode<AnimationPlayer>("AnimationPlayer").Play("Show");
    }

    
    public void HideMenu()
    { 
        this.GetNode<AnimationPlayer>("AnimationPlayer").Play("Hide");
        this.Visible =false;
    }    

    


}
