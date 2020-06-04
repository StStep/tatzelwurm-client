using Godot;
using System;
using System.Collections.Generic;

public class Battle: Node
{
    public enum BattleState { None, Deploying, Moving };

    private BattleState _state;
    private TurnGui _tg;
    private Battlefield _bf;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var dg = GetNode<DeployGui>("DeployGui");
        _tg = GetNode<TurnGui>("TurnGui");
        _bf = GetNode<Battlefield>("Battlefield");

        dg.CreateUnit = GetNode<Battlefield>("Battlefield").DeployUnit;
        _tg.Connect("finishedDeploying", this, nameof(EndDeployment));
        _bf.ValidityChanged += _ => OnBattleFieldChange();
        _bf.BusyChanged += _ => OnBattleFieldChange();

        EnterDeploymentState();
    }

    private void EndDeployment()
    {
        EnterMoveState();
    }

    private void EnterDeploymentState()
    {
        _state = BattleState.Deploying;
        GetNode<DeployGui>("DeployGui").Enabled = true;
    }

    private void EnterMoveState()
    {
        _state = BattleState.Moving;
        GetNode<DeployGui>("DeployGui").Enabled = false;
        GetNode<Battlefield>("Battlefield").Deploy2Move();
    }

    private void OnBattleFieldChange() => _tg.EnableEndTurn(!_bf.Busy && _bf.IsValid);
}
