using Godot;
using System;

public class MoveUnit : Node2D
{
    private enum State { Not_Selected, Idle, Add_Move_Single, Add_Move_Cont, Adjust_Move_Node }

    private Color C_NOT_SELECTED = new Color("ffffff"); // White
    private Color C_SELECTED = new  Color("f6ff00"); // Yellow
    private Color C_HIGHLIGHT = new  Color("b6ff00"); // Green-Yellow
    private Color C_PATH_NOT_SELECTED = new  Color("66ff68"); // Pastel-Green
    private Color C_PATH_SELECTED = new  Color("16ab19"); // Green
    private Color C_PATH_HIGHLIGHT = new  Color("b6ff00"); // Green-Yellow

    private PackedScene _move_node = GD.Load<PackedScene>("res://unit/position_node.tscn"); // Will load when the script is instanced.

    public SelectManager selManager;
    private State _state;
    private Color _markerCol;
    private Color _pathCol;

    private Sprite _ghost;
    private Sprite _start_marker_sprite;
    private Sprite _end_marker_sprite;
    private Line2D _move_prev;
    private MouseArea2d _start_marker;
    private MouseArea2d _end_marker;
    private SelectItem _select_item;

    // Tail of move node list
    private PositionNode _mv_tail = null;

    // First Move Node
    private PositionNode _mv_head = null;

    // Move Node Adjusting
    public PositionNode mv_adj = null;

    // Path Highlighting
    public PositionNode high_path = null;

    public Boolean is_selected => _select_item.is_selected;
    public Boolean is_busy => _select_item.is_busy;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _state = State.Not_Selected;
        _markerCol = C_NOT_SELECTED;
        _pathCol = C_PATH_NOT_SELECTED;

        _ghost = GetNode<Sprite>("Ghost");
        _start_marker_sprite = GetNode<Sprite>("StartMarker/Sprite");
        _end_marker_sprite = GetNode<Sprite>("EndMarker/Sprite");
        _move_prev = GetNode<Line2D>("MovePreview");
        _start_marker = GetNode<MouseArea2d>("StartMarker");
        _end_marker = GetNode<MouseArea2d>("EndMarker");
        _select_item = GetNode<SelectItem>("SelectItem");

        SetProcess(true);
        _start_marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(_render_marker_highlight), new Godot.Collections.Array() { _start_marker });
        _start_marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(_accept_event));
        _end_marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(_render_marker_highlight), new Godot.Collections.Array() { _end_marker });
        _end_marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(_accept_event));
        _select_item.Connect(nameof(SelectItem.selection_changed), this, nameof(_on_sel_change));
        _select_item.Connect(nameof(SelectItem.item_event_occured), this, nameof(_accept_event));
        _end_marker.Hide();
    }

    public override void _ExitTree()
    {
        if (is_selected)
        {
            selManager.req_selection(null);
        }
    }

    public override void _Process(float delta)
    {
        var mpos = _ghost.GetGlobalMousePosition();
        switch (_state)
        {
            case State.Idle:
                if (high_path != null && high_path.path_area.is_mouse_hovering)
                {
                    _ghost.Show();
                    _ghost.GlobalPosition = high_path.closest_pnt_on_path(mpos);
                }
                else
                {
                    _ghost.Hide();
                }
                break;
            case State.Add_Move_Cont:
                var end = _mv_tail != null ? _mv_tail.end : GlobalPosition;
                _move_prev.Points = new Vector2[] { ToLocal(end), ToLocal(mpos) };
                _ghost.GlobalPosition = mpos;
                _ghost.GlobalRotation = mpos.AngleToPoint(end) + Mathf.Pi/2f;
                break;
            case State.Adjust_Move_Node:
                if (mv_adj != null)
                {
                    mv_adj.end = mpos;
                    _end_marker.GlobalPosition = _mv_tail.end;
                }
                break;
            default:
                break;
        }
    }

    private void _accept_event(InputEvent ev)
    {
        if (handle_input(ev))
            GetTree().SetInputAsHandled();
    }

    private void _on_sel_change(Boolean is_sel)
    {
        if (is_sel)
            _change_state(State.Idle);
        else
            _change_state(State.Not_Selected);
    }

    private void _highlight()
    {
        _start_marker_sprite.Modulate = C_HIGHLIGHT;
        _end_marker_sprite.Modulate = C_HIGHLIGHT;
        var node = _mv_head;
        while (node != null)
        {
            node.path.Modulate = C_PATH_HIGHLIGHT;
            node = node.next;
        }
    }

    private void _unhighlight()
    {	_start_marker_sprite.Modulate = _markerCol;
        _end_marker_sprite.Modulate = _markerCol;
        var node = _mv_head;
        while (node != null)
        {
            node.path.Modulate = _pathCol;
            node = node.next;
        }
    }

    private void _render_marker_highlight(MouseArea2d marker)
    {	// Highlight everything if not yet selected
        if (_state == State.Not_Selected && marker.is_mouse_hovering && selManager.is_selection_allowed())
            _highlight();
        // If busy or not selected, don't highlight anything
        else if (_state == State.Not_Selected || is_busy)
            _unhighlight();
        // Else only highlight self
        else if (marker.is_mouse_hovering)
            marker.GetNode<Sprite>("Sprite").Modulate = C_HIGHLIGHT;
        else
            marker.GetNode<Sprite>("Sprite").Modulate = _markerCol;
    }

    private void _on_select()
    {
        _start_marker_sprite.Modulate = C_SELECTED;
        _end_marker_sprite.Modulate = C_SELECTED;
        _markerCol = C_SELECTED;
        _pathCol = C_PATH_SELECTED;
        var node = _mv_head;
        while (node != null)
        {
            node.enable();
            node.path.Modulate = C_PATH_SELECTED;
            node = node.next;
        }
        // Always hide last node under end marker
        _mv_tail?.disable();
        GD.Print("Selected " + GetName());
    }

    private void _on_deselect()
    {
        _start_marker_sprite.Modulate = C_NOT_SELECTED;
        _end_marker_sprite.Modulate = C_NOT_SELECTED;
        _markerCol = C_NOT_SELECTED;
        _pathCol = C_PATH_NOT_SELECTED;
        var node = _mv_head;
        while (node != null)
        {
            node.disable();
            node.path.Modulate = C_PATH_NOT_SELECTED;
            node = node.next;
        }
        GD.Print("Deselected " + Name);
    }

    private void _on_mv_reset()
    {
        _mv_tail = null;
        mv_adj = null;
        high_path = null;
        if (_mv_head != null)
        {
            _mv_head.erase();
            _mv_head = null;
        }
        _end_marker.Hide();
    }

    private void _change_state(State s)
    {
        if (s == _state)
            return;

        // Prev State
        switch (_state)
        {
            case State.Idle:
                _ghost.Hide();
                break;
            case State.Add_Move_Cont:
                _move_prev.Points = new Vector2[0];
                _ghost.Hide();
                break;
            default:
                break;
        }

        // New State
        switch (s)
        {
            case State.Not_Selected:
                _on_deselect();
                break;
            case State.Idle:
                _on_select();
                break;
            case State.Add_Move_Cont:
                _ghost.Show();
                break;
            default:
                break;
        }

        _state = s;
    }

    private void _add_move_node(Vector2 gpos)
    {
        GD.Print("Add move");
        var inst = _move_node.Instance() as PositionNode;
        inst.unit = this;
        AddChild(inst);

        // disable point under end marker, enable prev hidden
        if (_mv_head == null)
        {
            _mv_head = inst;
        }
        else
        {
            _mv_tail.enable();
            _mv_tail.next = inst;
        }
        inst.disable();
        inst.previous = _mv_tail;
        _mv_tail = inst;
        inst.end = gpos;
        inst.path.Modulate = C_PATH_SELECTED;

        // Move up end marker
        _end_marker.Show();
        _end_marker.GlobalPosition = _mv_tail.end;
        _end_marker.GlobalRotation = _mv_tail.GlobalRotation;
    }

    private void _rm_last_move_node()
    {
        mv_adj = null;
        high_path = null;
        var inst = _mv_tail.previous;
        _mv_tail.erase();
        _mv_tail = inst;
        if (_mv_tail != null)
        {
            _mv_tail.next = null;
            _mv_tail.disable();
            _end_marker.GlobalPosition = _mv_tail.end;
            _end_marker.GlobalRotation = _mv_tail.GlobalRotation;
        }
        else
        {
            _on_mv_reset();
        }
    }

    public Boolean handle_input(InputEvent ev)
    {
        var ret = false;
        switch(_state)
        {
            // select if hightlighted
           case State.Not_Selected:
                if ((_start_marker.is_mouse_hovering || _end_marker.is_mouse_hovering) &&
                    ev.IsActionPressed("ui_accept") && selManager.is_selection_allowed())
                {
                    selManager.req_selection(_select_item);
                    ret = true;
                }
                break;
            // Start adding moves if hightlighted or deselect
            case State.Idle:
                ret = true;
                if (_start_marker.is_mouse_hovering && ev.IsActionPressed("ui_accept"))
                {
                    _on_mv_reset();
                    _change_state(State.Add_Move_Cont);
                }
                else if (_end_marker.is_mouse_hovering && ev.IsActionPressed("ui_accept"))
                    _change_state(State.Add_Move_Cont);
                else if (mv_adj != null && mv_adj.marker.is_mouse_hovering && ev.IsActionPressed("ui_accept"))
                    _change_state(State.Adjust_Move_Node);
                else if (ev.IsActionPressed("ui_cancel"))
                {
                    if (selManager.is_selection_allowed())
                        selManager.req_selection(null);
                }
                else
                    ret = false;
                break;
            // Add Move or Return to Idle
            case State.Add_Move_Cont:
                ret = true;
                // Move end marker if click on, undo last
                if (ev.IsActionPressed("ui_accept") && _end_marker.is_mouse_hovering)
                    _rm_last_move_node();
                else if (ev.IsActionPressed("ui_accept"))
                    _add_move_node(_ghost.GlobalPosition);
                else if (ev.IsActionPressed("ui_cancel"))
                    _change_state(State.Idle);
                else
                    ret = false;
                break;
            // Return to idle once done, adj happens in _process()
            case State.Adjust_Move_Node:
                ret = true;
                if (ev.IsActionPressed("ui_accept") || ev.IsActionPressed("ui_cancel"))
                    _change_state(State.Idle);
                else
                    ret = false;
                break;
            default:
                break;
        }

        return ret;
    }
}
