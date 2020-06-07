using Godot;
using System;

public class PreBattle : CanvasLayer
{
    private Control _factionSelect;
    private Control _unitSelect;

    public override void _Ready()
    {
        _factionSelect = GetNode<Control>("FactionSelect");
        _factionSelect.GetNode<Button>("Control/Next").Connect("button_down", this, nameof(OnFactionSelectNext));

        _unitSelect = GetNode<Control>("UnitSelect");
        _unitSelect.GetNode<Button>("Control/Next").Connect("button_down", this, nameof(OnUnitSelectNext));

        _factionSelect.Show();
        _unitSelect.Hide();
    }

    private void OnFactionSelectNext()
    {
        _factionSelect.Hide();
        _unitSelect.Show();
    }

    private void OnUnitSelectNext()
    {
        GetTree().ChangeScene("res://Battle.tscn");
    }
}
