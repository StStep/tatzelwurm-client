using Godot;

public class TurnGui: CanvasLayer
{
    [Signal]
    public delegate void finishedDeploying();
    [Signal]
    public delegate void finishedTurn();

    public int Turn { get; private set; }

    private Button _buttonFinishDeploy;
    private Button _buttonFinishTurn;

    public override void _Ready()
    {
        _buttonFinishDeploy = GetNode<Button>("FinishDeploy");
        _buttonFinishTurn = GetNode<Button>("FinishTurn");
        _buttonFinishDeploy.Connect("button_down", this, nameof(OnFinishDeploy));
        _buttonFinishTurn.Connect("button_down", this, nameof(OnFinishTurn));
        _buttonFinishTurn.Visible = false;
    }

    private void OnFinishDeploy()
    {
        Turn = 1;
        GetNode("TurnHeader").Call("SetTurn", Turn);
        EmitSignal(nameof(finishedDeploying));
        _buttonFinishDeploy.Visible = false;
        _buttonFinishTurn.Visible = true;
    }

    private void OnFinishTurn()
    {
        if (Turn < 6)
        {
            Turn++;
            GetNode("TurnHeader").Call("SetTurn", Turn);
            if (Turn == 6)
                _buttonFinishTurn.Text = "Complete";
        }
        EmitSignal(nameof(finishedTurn));
    }
}