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
        _tg.Connect(nameof(TurnGui.finishedDeploying), this, nameof(EnterMoveState));
        _tg.Connect(nameof(TurnGui.finishedTurn), this, nameof(OnFinishTurn));
        _tg.Connect(nameof(TurnGui.nextTurn), this, nameof(OnNextTurn));
        _bf.ValidityChanged += _ => OnBattleFieldChange();
        _bf.BusyChanged += _ => OnBattleFieldChange();
        _bf.ActingDone += OnActingDone;

        EnterDeploymentState();
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

    private void OnFinishTurn()
    {
        _bf.ActTurn();
        _tg.EnableButton(false);
    }

    private void OnNextTurn()
    {
        if (_tg.Turn >= 6)
        {
            GetTree().ChangeScene("res://Start.tscn");
        }
        else
        {
            _tg.Turn++;
            _bf.NewTurn();
        }
    }

    private void OnBattleFieldChange() => _tg.EnableButton(!_bf.Busy && _bf.IsValid);

    private void OnActingDone() => _tg.EnableButton(true);
}
