using Godot;
using System;

public class TurnGui: CanvasLayer
{
    [Signal]
    public delegate void finishedDeploying();
    [Signal]
    public delegate void finishedTurn();
    [Signal]
    public delegate void nextTurn();

    private int __Turn;
    public int Turn
    {
        get => __Turn;
        set
        {
            __Turn = value > 6 || value < 1 ? 6 : value;
            GetNode("TurnHeader").Call("SetTurn", Turn);
            if (Turn == 6)
                _buttonNextTurn.Text = "Complete";
        }
    }

    private Button _buttonFinishDeploy;
    private Button _buttonFinishTurn;
    private Button _buttonNextTurn;

    public override void _Ready()
    {
        _buttonFinishDeploy = GetNode<Button>("FinishDeploy");
        _buttonFinishDeploy.Connect("button_down", this, nameof(OnFinishDeploy));

        _buttonFinishTurn = GetNode<Button>("FinishTurn");
        _buttonFinishTurn.Connect("button_down", this, nameof(OnFinishTurn));
        _buttonFinishTurn.Visible = false;

        _buttonNextTurn = GetNode<Button>("NextTurn");
        _buttonNextTurn.Connect("button_down", this, nameof(OnNextTurn));
        _buttonNextTurn.Visible = false;
    }

    public void EnableButton(Boolean en)
    {
        _buttonFinishDeploy.Disabled = !en;
        _buttonFinishTurn.Disabled = !en;
        _buttonNextTurn.Disabled = !en;
    }

    private void OnFinishDeploy()
    {
        Turn = 1;
        GetNode("TurnHeader").Call("SetTurn", Turn);
        _buttonFinishDeploy.Visible = false;
        _buttonFinishTurn.Visible = true;
        _buttonNextTurn.Visible = false;
        EmitSignal(nameof(finishedDeploying));
    }

    private void OnFinishTurn()
    {
        _buttonFinishDeploy.Visible = false;
        _buttonFinishTurn.Visible = false;
        _buttonNextTurn.Visible = true;
        EmitSignal(nameof(finishedTurn));
    }

    private void OnNextTurn()
    {
        _buttonFinishDeploy.Visible = false;
        _buttonFinishTurn.Visible = true;
        _buttonNextTurn.Visible = false;
        EmitSignal(nameof(nextTurn));
    }
}