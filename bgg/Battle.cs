using Godot;
using System;
using System.Collections.Generic;

public class Battle: Node
{
    public enum BattleState { None, Deploying, Moving };

    private BattleState _state;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
	    GetNode("DeployGui").Set("create_unit", GD.FuncRef(GetNode("Battlefield"), nameof(Battlefield.DeployUnit)));
	    GetNode("TurnGui").Connect("finishedDeploying", this, nameof(EndDeployment));

        EnterDeploymentState();
    }

    private void EndDeployment()
    {
        EnterMoveState();
    }

    private void EnterDeploymentState()
    {
        _state = BattleState.Deploying;
	    GetNode("DeployGui").Call("enable");
    }

    private void EnterMoveState()
    {
        _state = BattleState.Moving;
	    GetNode("DeployGui").Call("disable");
        GetNode<Battlefield>("Battlefield").Deploy2Move();
    }
}
