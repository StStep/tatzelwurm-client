using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MoveUnit : Node2D
{
    private enum State { NotSelected, Idle, AddingNodes, AdjustingNode }

    private Color colNotSelected = new Color("ffffff"); // White
    private Color colSelected = new  Color("f6ff00"); // Yellow
    private Color colHighlight = new  Color("b6ff00"); // Green-Yellow
    private Color colPathNotSelected = new  Color("66ff68"); // Pastel-Green
    private Color colPathSelected = new  Color("16ab19"); // Green
    private Color colPathHighlight = new  Color("b6ff00"); // Green-Yellow

    private PackedScene _posNodeScene = GD.Load<PackedScene>("res://units/PositionNode.tscn"); // Will load when the script is instanced.

    private State _state;
    private Color _prevMarkerCol;
    private Color _prevPathCol;

    private Node2D _ghost;
    private Sprite _start_marker_sprite;
    private Sprite _end_marker_sprite;
    private Line2D _move_prev;
    private MouseArea2d _start_marker;
    private MouseArea2d _end_marker;
    private SelectItem _selectItem;

    // Tail of move node list
    private PositionNode _nodeTail;

    // First Move Node
    private PositionNode _nodeHead;


    public SelectManager SelectManager { get; set; }

    // Move Node Adjusting
    public PositionNode AdjustingNode { get; set; }

    // Path Highlighting
    public PositionNode HighlightedPathNode { get; set; }

    public Boolean IsSelected => _selectItem.IsSelected;
    public Boolean IsBusy => _selectItem.IsBusy;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _state = State.NotSelected;
        _prevMarkerCol = colNotSelected;
        _prevPathCol = colPathNotSelected;

        _ghost = GetNode<Node2D>("Ghost");
        _start_marker_sprite = GetNode<Sprite>("StartMarker/Sprite");
        _end_marker_sprite = GetNode<Sprite>("EndMarker/Sprite");
        _move_prev = GetNode<Line2D>("MovePreview");
        _start_marker = GetNode<MouseArea2d>("StartMarker");
        _end_marker = GetNode<MouseArea2d>("EndMarker");
        _selectItem = GetNode<SelectItem>("SelectItem");

        SetProcess(true);
        _start_marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange), new Godot.Collections.Array() { _start_marker });
        _start_marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(OnMarkerEvent));
        _end_marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange), new Godot.Collections.Array() { _end_marker });
        _end_marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(OnMarkerEvent));
        _selectItem.Connect(nameof(SelectItem.selection_changed), this, nameof(OnSelectionChange));
        _end_marker.Hide();
    }

    public override void _ExitTree()
    {
        if (IsSelected)
        {
            SelectManager.ReqSelection(null);
        }
    }

    public override void _Process(float delta)
    {
        var mpos = _ghost.GetGlobalMousePosition();
        switch (_state)
        {
            case State.Idle:
                if (HighlightedPathNode != null && HighlightedPathNode.PathArea.is_mouse_hovering)
                {
                    _ghost.Show();
                    _ghost.GlobalPosition = HighlightedPathNode.GetClosestPointOnPath(GetNodeStartGpos(HighlightedPathNode), mpos);
                }
                else
                {
                    _ghost.Hide();
                }
                break;
            case State.AddingNodes:
                MoveGhost(mpos);
                break;
            case State.AdjustingNode:
                if (AdjustingNode != null)
                {
                    var end = mpos;
                    var start = GetNodeStartGpos(AdjustingNode);
                    AdjustingNode.GlobalPosition = end;
                    AdjustingNode.GlobalRotation = end.AngleToPoint(start);
                    AdjustingNode.SetAsLine(start, end);
                    _end_marker.GlobalPosition = _nodeTail.GlobalPosition;
                    _end_marker.GlobalRotation = _nodeTail.GlobalRotation;
                    SetMoveIndicator(_nodeTail.GlobalPosition, _nodeTail.GlobalRotation);
                }
                break;
            case State.NotSelected:
            default:
                break;
        }
    }

    public override void _Input(InputEvent @event)
    {
        switch(_state)
        {
             // Start adding moves if hightlighted or deselect
            case State.Idle:
                if (_start_marker.is_mouse_hovering && @event.IsActionPressed("ui_accept"))
                {
                    ResetMoveNodes();
                    ChangeState(State.AddingNodes);
                }
                else if (_end_marker.is_mouse_hovering && @event.IsActionPressed("ui_accept"))
                {
                    ChangeState(State.AddingNodes);
                }
                else if (@event.IsActionPressed("ui_cancel"))
                {
                    if (SelectManager.IsSelectionAllowed())
                        SelectManager.ReqSelection(null);
                    GetTree().SetInputAsHandled();
                }
                break;
            // Add Move or Return to Idle
            case State.AddingNodes:
                // Move end marker if click on, undo last
                if (@event.IsActionPressed("ui_accept") && _end_marker.is_mouse_hovering)
                {
                    RmLastMoveNode();
                    GetTree().SetInputAsHandled();
                }
                else if (@event.IsActionPressed("ui_accept"))
                {
                    AddMoveNode(_ghost.GlobalPosition, Vector2.Right.Rotated(_ghost.GlobalRotation), false, Enumerable.Empty<String>());
                    GetTree().SetInputAsHandled();
                }
                else if (@event.IsActionPressed("ui_cancel"))
                {
                    ChangeState(State.Idle);
                    GetTree().SetInputAsHandled();
                }
                break;
            // Return to idle once done, adj happens in _process()
            case State.AdjustingNode:
                if (@event.IsActionPressed("ui_accept") || @event.IsActionPressed("ui_cancel"))
                {
                    ChangeState(State.Idle);
                    GetTree().SetInputAsHandled();
                }
                break;
            default:
                break;
        }
    }

    private void OnMarkerEvent(InputEvent @event)
    {
        if (_state == State.NotSelected && @event.IsActionPressed("ui_accept") && SelectManager.IsSelectionAllowed())
        {
            SelectManager.ReqSelection(_selectItem);
            GetTree().SetInputAsHandled();
        }
    }

    private void OnChildNodeEvent(PositionNode node, InputEvent @event)
    {
        if (_state == State.NotSelected && @event.IsActionPressed("ui_accept") && SelectManager.IsSelectionAllowed())
        {
            SelectManager.ReqSelection(_selectItem);
            GetTree().SetInputAsHandled();
        }
        else if (_state == State.Idle && @event.IsActionPressed("ui_accept"))
        {
            AdjustingNode = node;
            ChangeState(State.AdjustingNode);
            GetTree().SetInputAsHandled();
        }
    }

    private void OnChildNodeHover(PositionNode node, Boolean hovering)
    {
        if (SelectManager?.IsSelectionAllowed() == false)
        { }
        else if (hovering)
        {
            node.highlight_body("Focus");
            if (!IsSelected)
            {
                HighlightEverything();
            }
        }
        else
        {
            node.highlight_body("None");
            if (!IsSelected)
            {
                UnhighlightEverything();
            }
        }
    }

    private void OnSelectionChange(Boolean isSelected) => ChangeState(isSelected ? State.Idle : State.NotSelected);

    private void OnMarkerHoverChange(MouseArea2d marker)
    {
        // Do nothing if busy
        if (IsBusy)
        {
            return;
        }

        // Highlight everything if not yet selected and selection allowed
        if (_state == State.NotSelected && marker.is_mouse_hovering && SelectManager.IsSelectionAllowed())
        {
            HighlightEverything();
        }
        // If not selected, don't highlight anything or busy
        else if (_state == State.NotSelected)
        {
            UnhighlightEverything();
        }
        // Else only highlight given marker
        else if (marker.is_mouse_hovering)
        {
            marker.GetNode<Sprite>("Sprite").Modulate = colHighlight;
        }
        else
        {
            marker.GetNode<Sprite>("Sprite").Modulate = _prevMarkerCol;
        }
    }

    private void MoveGhost(Vector2 mpos)
    {
        var gpos = _nodeTail != null ? _nodeTail.GlobalPosition : GlobalPosition;
        var grot = _nodeTail != null ? _nodeTail.GlobalRotation : GlobalRotation;
        var dir = Vector2.Right.Rotated(grot);
        var quarter = Trig.GetQuarter(gpos, dir, mpos, 68f, 32.5f);
        if (quarter == Trig.Quarter.front && Trig.DistToLine(new Trig.Ray2(gpos, dir), mpos) > 20f)
        {
            try
            {
                var arc = new Trig.Arc2(gpos, grot, mpos);
                _move_prev.Points = Trig.SampleArc(arc, 20).Select(s => ToLocal(s)).ToArray();
                _ghost.GlobalPosition = arc.End;
                _ghost.GlobalRotation = arc.EndDir.Angle();
            }
            catch
            {
                GD.Print($"Failed to make arc with {gpos} {grot} {mpos}");
            }
        }
        else if (quarter == Trig.Quarter.left || quarter == Trig.Quarter.right)
        {
            var endGpos = Trig.NearestPointOnLine(new Trig.Ray2(gpos, dir.Rotated(Mathf.Pi/2f)), mpos);
            _move_prev.Points = new Vector2[] { ToLocal(gpos), ToLocal(endGpos) };
            _ghost.GlobalPosition = endGpos;
            _ghost.GlobalRotation = grot;
        }
        else
        {
            var endGpos = Trig.NearestPointOnLine(new Trig.Ray2(gpos, dir), mpos);
            _move_prev.Points = new Vector2[] { ToLocal(gpos), ToLocal(endGpos) };
            _ghost.GlobalPosition = endGpos;
            _ghost.GlobalRotation = grot;
        }
    }

    private void HighlightEverything()
    {
        _start_marker_sprite.Modulate = colHighlight;
        _end_marker_sprite.Modulate = colHighlight;
        for(var node = _nodeHead; node != null; node = node.Next)
        {
            node.Path.Modulate = colPathHighlight;
            node.highlight_body("Focus");
        }
    }

    private void UnhighlightEverything()
    {	_start_marker_sprite.Modulate = _prevMarkerCol;
        _end_marker_sprite.Modulate = _prevMarkerCol;
        for(var node = _nodeHead; node != null; node = node.Next)
        {
            node.Path.Modulate = _prevPathCol;
            node.highlight_body("None");
        }
    }

    private void ChangeState(State s)
    {
        if (s == _state)
            return;

        // Prev State
        switch (_state)
        {
            case State.Idle:
                _ghost.Hide();
                break;
            case State.AddingNodes:
                _move_prev.Points = new Vector2[0];
                _ghost.Hide();
                break;
            case State.AdjustingNode:
                for(var node = _nodeHead; node != null; node = node.Next)
                    node.Enable();
                break;
            default:
                break;
        }

        // New State
        _selectItem.IsBusy = true;
        switch (s)
        {
            case State.NotSelected:
                GD.Print("To State NotSelected " + Name);
                HandleDeselect();
                _selectItem.IsBusy = false;
                break;
            case State.Idle:
                GD.Print("To State Idle " + Name);
                HandleSelect();
                _selectItem.IsBusy = false;
                break;
            case State.AddingNodes:
                GD.Print("To State AddingNodes " + Name);
                _ghost.Show();
                break;
            case State.AdjustingNode:
                GD.Print("To State AdjustingNode " + Name);
                for(var node = _nodeHead; node != null; node = node.Next)
                    node.Disable();
                AdjustingNode.Enable();
                break;
            default:
                break;
        }

        _state = s;
    }

    private void HandleSelect()
    {
        SetProcessInput(true);
        _start_marker_sprite.Modulate = colSelected;
        _end_marker_sprite.Modulate = colSelected;
        _prevMarkerCol = colSelected;
        _prevPathCol = colPathSelected;
        for(var node = _nodeHead; node != null; node = node.Next)
        {
            node.Path.Modulate = colPathSelected;
            if (!node.Body.is_mouse_hovering)
                node.highlight_body("None");
        }
    }

    private void HandleDeselect()
    {
        SetProcessInput(false);
        _start_marker_sprite.Modulate = colNotSelected;
        _end_marker_sprite.Modulate = colNotSelected;
        _prevMarkerCol = colNotSelected;
        _prevPathCol = colPathNotSelected;
        for(var node = _nodeHead; node != null; node = node.Next)
        {
            node.Path.Modulate = colPathNotSelected;
            if (!node.Body.is_mouse_hovering)
                node.highlight_body("None");
        }
    }

    public PositionNode AddMoveNode(Vector2 gpos, Vector2 dir, bool arc, IEnumerable<String> annotations)
    {
        GD.Print("Add move");
        var inst = _posNodeScene.Instance() as PositionNode;
        inst.Connect(nameof(PositionNode.event_on_hover), this, nameof(OnChildNodeEvent));
        inst.Connect(nameof(PositionNode.marker_hover), this, nameof(OnChildNodeHover));

        // disable point under end marker, enable prev hidden
        if (_nodeHead == null)
        {
            _nodeHead = inst;
            AddChild(inst);
        }
        else
        {
            _nodeTail.Enable();
            _nodeTail.Sprite.Show();
            _nodeTail.Next = inst;
            _nodeTail.AddChild(inst);
        }

        inst.Disable();
        inst.Sprite.Hide();
        inst.Previous = _nodeTail;
        _nodeTail = inst;

        _nodeTail.GlobalPosition = gpos;
        var start = GetNodeStartGpos(_nodeTail);
        if (arc)
        {
            var a = new Trig.Arc2(start, GetNodeStartGrot(_nodeTail), gpos);
            var angle = dir != Vector2.Zero ? dir.Angle() : a.EndDir.Angle();
            _nodeTail.GlobalRotation = angle;
            _nodeTail.SetAsArc(a);
        }
        else
        {
            var angle = dir != Vector2.Zero ? dir.Angle() : (gpos - start).Angle();
            _nodeTail.GlobalRotation = angle;
            _nodeTail.SetAsLine(start, gpos);
        }
        _nodeTail.Path.Modulate = IsSelected ? colPathSelected : colPathNotSelected;

        // Move up end marker
        _end_marker.Show();
        _end_marker.GlobalPosition = _nodeTail.GlobalPosition;
        _end_marker.GlobalRotation = _nodeTail.GlobalRotation;
        SetMoveIndicator(_nodeTail.GlobalPosition, _nodeTail.GlobalRotation);

        foreach (var anno in annotations)
        {
            _nodeTail.add_annotation(anno);
        }

        return _nodeTail;
    }

    public void RmLastMoveNode()
    {
        AdjustingNode = null;
        HighlightedPathNode = null;
        var inst = _nodeTail.Previous;
        _nodeTail.QueueFree();
        _nodeTail = inst;
        if (_nodeTail != null)
        {
            _nodeTail.Next = null;
            _nodeTail.Disable();
            _nodeTail.Sprite.Hide();
            _end_marker.GlobalPosition = _nodeTail.GlobalPosition;
            _end_marker.GlobalRotation = _nodeTail.GlobalRotation;
            SetMoveIndicator(_nodeTail.GlobalPosition, _nodeTail.GlobalRotation);
        }
        else
        {
            ResetMoveNodes();
            SetMoveIndicator(GlobalPosition, GlobalRotation);
        }
    }

    public void ResetMoveNodes()
    {
        _nodeTail = null;
        AdjustingNode = null;
        HighlightedPathNode = null;
        SetMoveIndicator(GlobalPosition, GlobalRotation);
        SetMoveIndicatorVisibility(false);

        var inst = _nodeHead;
        while (inst != null)
        {
            inst.QueueFree();
            inst = inst.Next;
        }
        _nodeHead = null;
        _end_marker.Hide();
    }

    public void SetMoveIndicatorVisibility(bool visible)
    {
        GetNode<Node2D>("MoveIndicator").Visible = visible;
    }

    private void SetMoveIndicator(Vector2 gpos, float grot)
    {
        var n = GetNode<Node2D>("MoveIndicator");
        n.GlobalPosition = gpos;
        n.GlobalRotation = grot;
    }

    public void highlight_body(Godot.Object o)
    {

    }

    public Vector2 GetNodeStartGpos(PositionNode n) => n.Previous != null ? n.Previous.GlobalPosition : GlobalPosition;
    public float GetNodeStartGrot(PositionNode n) => n.Previous != null ? n.Previous.GlobalRotation : GlobalRotation;
}
