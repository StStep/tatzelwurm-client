using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class ActionUnit : Area2D
{
    private List<MoveCommand> _cmdList;
    private int _cmdInd = 0;

    public MovementState CurrentState { get; private set; }
    public MoveCommand CurrentCommand => (_cmdInd < _cmdList.Count && _cmdInd >= 0) ?  _cmdList?.ElementAt(_cmdInd) : null;
    public Boolean Active { get; private set; }
    public float Time { get; private set; }

    public override void _Ready()
    {

    }

    private void Next()
    {
        if (_cmdInd + 1 >= _cmdList.Count())
        {
            Active = false;
        }
        else
        {
            _cmdInd++;
        }
    }

    public void Act(float delta)
    {
        if (!Active || CurrentCommand == null)
            return;

        Time += delta;
        CurrentCommand.Update(CurrentState, delta);
        SyncState();
        if (Time >= CurrentCommand.Period)
        {
            Time = 0;
            Next();
        }
    }

    public void Restart(IEnumerable<MoveCommand> cmdList)
    {
        _cmdList = cmdList.ToList();
        _cmdInd = 0;
        Active = true;
        Time = 0f;
        if (CurrentCommand != null)
        {
            CurrentState = CurrentCommand.Initial.Clone();
            SyncState();
        }
    }

    public void SyncState()
    {
        GlobalPosition = CurrentState.Position;
        GlobalRotation = CurrentState.Rotation;
    }
}
