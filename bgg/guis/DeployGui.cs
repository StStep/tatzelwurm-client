using Godot;
using System;
using System.Collections.Generic;

public class DeployGui: Node
{
    public Boolean Enabled
    {
        get => GetNode<CanvasItem>("UnitPalette").Visible;
        set => GetNode<CanvasItem>("UnitPalette").Visible = value;
    }

    public Action<String> CreateUnit { get; set; }
    public int LineCount { get; private set; } = 6;
    public int TroopCount { get; private set; } = 4;
    public int RegimentCount { get; private set; } = 2;

    private Label _labelLineCount;
    private Label _labelTroopCount;
    private Label _labelRegimentCount;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _labelLineCount = GetNode<Label>("UnitPalette/MarginContainer/VBoxContainer/HBoxContainer/LineCount");
        _labelTroopCount  = GetNode<Label>("UnitPalette/MarginContainer/VBoxContainer/HBoxContainer2/TroopCount");
        _labelRegimentCount  = GetNode<Label>("UnitPalette/MarginContainer/VBoxContainer/HBoxContainer3/RegimentCount");
        GetNode("UnitPalette/MarginContainer/VBoxContainer/HBoxContainer/BtMakeLine").Connect("button_down", this, nameof(OnPressMakeLine));
        GetNode("UnitPalette/MarginContainer/VBoxContainer/HBoxContainer2/BtMakeTroop").Connect("button_down", this, nameof(OnPressMakeTroop));
        GetNode("UnitPalette/MarginContainer/VBoxContainer/HBoxContainer3/BtMakeRegiment").Connect("button_down", this, nameof(OnPressMakeRegiment));
        _labelLineCount.Text = LineCount.ToString();
        _labelTroopCount.Text = TroopCount.ToString();
        _labelRegimentCount.Text = RegimentCount.ToString();
    }

    private void OnPressMakeLine()
    {
        if (!Enabled || LineCount <= 0)
            return;
        CreateUnit?.Invoke("line");
        LineCount -= 1;
        _labelLineCount.Text = LineCount.ToString();
    }

    private void OnPressMakeTroop()
    {
        if (!Enabled || TroopCount <= 0)
            return;
        CreateUnit?.Invoke("troop");
        TroopCount -= 1;
        _labelTroopCount.Text = TroopCount.ToString();
    }

    private void OnPressMakeRegiment()
    {
        if (!Enabled || RegimentCount <= 0)
            return;
        CreateUnit?.Invoke("regiment");
        RegimentCount -= 1;
        _labelRegimentCount.Text = RegimentCount.ToString();
    }
}
