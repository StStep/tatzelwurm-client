using Godot;
using System;

public class MoveUnit : Node2D
{
    private enum State { NotSelected, Idle, AddingNodes, AdjustingNode }

    private Color colNotSelected = new Color("ffffff"); // White
    private Color colSelected = new  Color("f6ff00"); // Yellow
    private Color colHighlight = new  Color("b6ff00"); // Green-Yellow
    private Color colPathNotSelected = new  Color("66ff68"); // Pastel-Green
    private Color colPathSelected = new  Color("16ab19"); // Green
    private Color colPathHighlight = new  Color("b6ff00"); // Green-Yellow

    private PackedScene _posNodeScene = GD.Load<PackedScene>("res://unit/position_node.tscn"); // Will load when the script is instanced.

    private State _state;
    private Color _prevMarkerCol;
    private Color _prevPathCol;

    private Sprite _ghost;
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

        _ghost = GetNode<Sprite>("Ghost");
        _start_marker_sprite = GetNode<Sprite>("StartMarker/Sprite");
        _end_marker_sprite = GetNode<Sprite>("EndMarker/Sprite");
        _move_prev = GetNode<Line2D>("MovePreview");
        _start_marker = GetNode<MouseArea2d>("StartMarker");
        _end_marker = GetNode<MouseArea2d>("EndMarker");
        _selectItem = GetNode<SelectItem>("SelectItem");

        SetProcess(true);
        _start_marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange), new Godot.Collections.Array() { _start_marker });
        _start_marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(OnEvent));
        _end_marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange), new Godot.Collections.Array() { _end_marker });
        _end_marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(OnEvent));
        _selectItem.Connect(nameof(SelectItem.selection_changed), this, nameof(OnSelectionChange));
        _selectItem.Connect(nameof(SelectItem.item_event_occured), this, nameof(OnEvent));
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
                if (HighlightedPathNode != null && HighlightedPathNode.path_area.is_mouse_hovering)
                {
                    _ghost.Show();
                    _ghost.GlobalPosition = HighlightedPathNode.closest_pnt_on_path(mpos);
                }
                else
                {
                    _ghost.Hide();
                }
                break;
            case State.AddingNodes:
                var end = _nodeTail != null ? _nodeTail.end : GlobalPosition;
                _move_prev.Points = new Vector2[] { ToLocal(end), ToLocal(mpos) };
                _ghost.GlobalPosition = mpos;
                _ghost.GlobalRotation = mpos.AngleToPoint(end) + Mathf.Pi/2f;
                break;
            case State.AdjustingNode:
                if (AdjustingNode != null)
                {
                    AdjustingNode.end = mpos;
                    _end_marker.GlobalPosition = _nodeTail.end;
                }
                break;
            case State.NotSelected:
            default:
                break;
        }
    }

    private void OnEvent(InputEvent ev)
    {
        if (HandleInput(ev))
        {
            GetTree().SetInputAsHandled();
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

    private void HighlightEverything()
    {
        _start_marker_sprite.Modulate = colHighlight;
        _end_marker_sprite.Modulate = colHighlight;
        var node = _nodeHead;
        while (node != null)
        {
            node.path.Modulate = colPathHighlight;
            node = node.next;
        }
    }

    private void UnhighlightEverything()
    {	_start_marker_sprite.Modulate = _prevMarkerCol;
        _end_marker_sprite.Modulate = _prevMarkerCol;
        var node = _nodeHead;
        while (node != null)
        {
            node.path.Modulate = _prevPathCol;
            node = node.next;
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
            default:
                break;
        }

        // New State
        _selectItem.IsBusy = true;
        switch (s)
        {
            case State.NotSelected:
                HandleDeselect();
                _selectItem.IsBusy = false;
                break;
            case State.Idle:
                HandleSelect();
                _selectItem.IsBusy = false;
                break;
            case State.AddingNodes:
                _ghost.Show();
                break;
            case State.AdjustingNode:
            default:
                break;
        }

        _state = s;
    }

    private void HandleSelect()
    {
        _start_marker_sprite.Modulate = colSelected;
        _end_marker_sprite.Modulate = colSelected;
        _prevMarkerCol = colSelected;
        _prevPathCol = colPathSelected;
        var node = _nodeHead;
        while (node != null)
        {
            node.enable();
            node.path.Modulate = colPathSelected;
            node = node.next;
        }

        // Always hide last node under end marker
        _nodeTail?.disable();
        GD.Print("Selected " + GetName());
    }

    private void HandleDeselect()
    {
        _start_marker_sprite.Modulate = colNotSelected;
        _end_marker_sprite.Modulate = colNotSelected;
        _prevMarkerCol = colNotSelected;
        _prevPathCol = colPathNotSelected;
        var node = _nodeHead;
        while (node != null)
        {
            node.disable();
            node.path.Modulate = colPathNotSelected;
            node = node.next;
        }

        GD.Print("Deselected " + Name);
    }

    public Boolean HandleInput(InputEvent ev)
    {
        var ret = false;
        switch(_state)
        {
            // select if hightlighted and not yet selected
           case State.NotSelected:
                if ((_start_marker.is_mouse_hovering || _end_marker.is_mouse_hovering) &&
                    ev.IsActionPressed("ui_accept") && SelectManager.IsSelectionAllowed())
                {
                    SelectManager.ReqSelection(_selectItem);
                    ret = true;
                }
                break;
            // Start adding moves if hightlighted or deselect
            case State.Idle:
                ret = true;
                if (_start_marker.is_mouse_hovering && ev.IsActionPressed("ui_accept"))
                {
                    ResetMoveNodes();
                    ChangeState(State.AddingNodes);
                }
                else if (_end_marker.is_mouse_hovering && ev.IsActionPressed("ui_accept"))
                {
                    ChangeState(State.AddingNodes);
                }
                else if (AdjustingNode != null && AdjustingNode.marker.is_mouse_hovering && ev.IsActionPressed("ui_accept"))
                {
                    ChangeState(State.AdjustingNode);
                }
                else if (ev.IsActionPressed("ui_cancel"))
                {
                    if (SelectManager.IsSelectionAllowed())
                        SelectManager.ReqSelection(null);
                }
                else
                    ret = false;
                break;
            // Add Move or Return to Idle
            case State.AddingNodes:
                ret = true;
                // Move end marker if click on, undo last
                if (ev.IsActionPressed("ui_accept") && _end_marker.is_mouse_hovering)
                {
                    RmLastMoveNode();
                }
                else if (ev.IsActionPressed("ui_accept"))
                {
                    AddMoveNode(_ghost.GlobalPosition);
                }
                else if (ev.IsActionPressed("ui_cancel"))
                {
                    ChangeState(State.Idle);
                }
                else
                {
                    ret = false;
                }
                break;
            // Return to idle once done, adj happens in _process()
            case State.AdjustingNode:
                ret = true;
                if (ev.IsActionPressed("ui_accept") || ev.IsActionPressed("ui_cancel"))
                {
                    ChangeState(State.Idle);
                }
                else
                {
                    ret = false;
                }
                break;
            default:
                break;
        }

        return ret;
    }

    private void AddMoveNode(Vector2 gpos)
    {
        GD.Print("Add move");
        var inst = _posNodeScene.Instance() as PositionNode;
        inst.unit = this;
        AddChild(inst);

        // disable point under end marker, enable prev hidden
        if (_nodeHead == null)
        {
            _nodeHead = inst;
        }
        else
        {
            _nodeTail.enable();
            _nodeTail.next = inst;
        }
        inst.disable();
        inst.previous = _nodeTail;
        _nodeTail = inst;
        inst.end = gpos;
        inst.path.Modulate = colPathSelected;

        // Move up end marker
        _end_marker.Show();
        _end_marker.GlobalPosition = _nodeTail.end;
        _end_marker.GlobalRotation = _nodeTail.GlobalRotation;
    }

    private void RmLastMoveNode()
    {
        AdjustingNode = null;
        HighlightedPathNode = null;
        var inst = _nodeTail.previous;
        _nodeTail.erase();
        _nodeTail = inst;
        if (_nodeTail != null)
        {
            _nodeTail.next = null;
            _nodeTail.disable();
            _end_marker.GlobalPosition = _nodeTail.end;
            _end_marker.GlobalRotation = _nodeTail.GlobalRotation;
        }
        else
        {
            ResetMoveNodes();
        }
    }

    private void ResetMoveNodes()
    {
        _nodeTail = null;
        AdjustingNode = null;
        HighlightedPathNode = null;
        if (_nodeHead != null)
        {
            _nodeHead.erase();
            _nodeHead = null;
        }
        _end_marker.Hide();
    }
}
