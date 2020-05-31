using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum MoveType { None, March, Reposition, Wheel, Rotate };

public class MoveUnit : Node2D, IUnit
{
    private enum State { NotSelected, Idle, AddingNodes, RotatingNode, AdjustingNode }

    private Color colNotSelected = new Color("ffffff"); // White
    private Color colSelected = new Color("f6ff00"); // Yellow
    private Color colHighlight = new Color("b6ff00"); // Green-Yellow
    private Color colPathNotSelected = new Color("66ff68"); // Pastel-Green
    private Color colPathSelected = new Color("16ab19"); // Green
    private Color colPathHighlight = new Color("b6ff00"); // Green-Yellow
    private Color colPathAdding = new Color("6680ff"); // Pale-Blue
    private Color colPathError = new Color("ff1a1a"); // Pale-Red

    private PackedScene _posNodeScene = GD.Load<PackedScene>("res://units/PositionNode.tscn"); // Will load when the script is instanced.

    private State _state;
    private Color _prevMarkerCol;
    private Color _prevPathCol;

    private Node2D _ghost;
    private MoveType _moveType;
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

    private int _lastDragInd = -1;


    public SelectManager SelectManager { get; set; }

    // Move Node Adjusting
    private PositionNode __adjNode;
    private Vector2 _adjNodeOrigGpos;
    private float _adjNodeOrigGrot;
    public PositionNode AdjustingNode
    {
        get => __adjNode;
        set
        {
            _adjNodeOrigGpos = value?.GlobalPosition ?? Vector2.Zero;
            _adjNodeOrigGrot = value?.GlobalRotation ?? 0f;
            __adjNode = value;
        }
    }

    // Path Highlighting
    public PositionNode HighlightedPathNode { get; set; }

    public Boolean IsSelected => _selectItem.IsSelected;
    public Boolean IsBusy => _selectItem.IsBusy;
    public Boolean Valid { get; set; }

    public int MoveNodeCount { get; private set; } = 0;

    public int MoveNodeLimit { get; set; } = 4;

    public float MaxFwdDistance { get; set; } = 500f;
    public float MaxSideDistance { get; set; } = 250f;
    public float MaxRvrDistance { get; set; } = 125f;


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
        _start_marker.Connect(nameof(MouseArea2d.mouse_drag_updated), this, nameof(OnMarkerDrag), new Godot.Collections.Array() { false, _start_marker });
        _start_marker.Connect(nameof(MouseArea2d.mouse_drag_ended), this, nameof(OnMarkerDrag), new Godot.Collections.Array() { true, _start_marker });
        _start_marker.Connect(nameof(MouseArea2d.mouse_clicked), this, nameof(OnClickStart));
        _end_marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange), new Godot.Collections.Array() { _end_marker });
        _end_marker.Connect(nameof(MouseArea2d.mouse_drag_updated), this, nameof(OnMarkerDrag), new Godot.Collections.Array() { false, _end_marker });
        _end_marker.Connect(nameof(MouseArea2d.mouse_drag_ended), this, nameof(OnMarkerDrag), new Godot.Collections.Array() { true, _end_marker });
        _end_marker.Connect(nameof(MouseArea2d.mouse_clicked), this, nameof(OnClickEnd));
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
                if (!MoveNode(_ghost, GetTailGpos(), GetTailGrot(), mpos))
                {
                    _ghost.GlobalPosition = GetTailGpos();
                    _ghost.GlobalRotation = GetTailGrot();
                    _move_prev.Modulate = colPathError;
                    _move_prev.Points = new Vector2[] { ToLocal(_ghost.GlobalPosition), ToLocal(mpos) };
                }
                break;
            case State.AdjustingNode:
                if (AdjustingNode != null)
                {
                    if (!MoveNode(AdjustingNode, GetNodeStartGpos(AdjustingNode), GetNodeStartGrot(AdjustingNode), mpos))
                    {
                        AdjustingNode.GlobalPosition = _adjNodeOrigGpos;
                        AdjustingNode.GlobalRotation = _adjNodeOrigGrot;
                        _move_prev.Modulate = colPathError;
                        _move_prev.Points = new Vector2[] { ToLocal(GetNodeStartGpos(AdjustingNode)), ToLocal(mpos) };
                        AdjustingNode.Sprite.Show();
                        _ghost.Hide();
                    }
                    else
                    {
                        _ghost.GlobalPosition = AdjustingNode.GlobalPosition;
                        _ghost.GlobalRotation = AdjustingNode.GlobalRotation;
                        __adjNode.Sprite.Hide();
                        _ghost.Show();
                    }
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
            case State.Idle:
                if (@event.IsActionReleased("ui_cancel"))
                {
                    if (SelectManager.IsSelectionAllowed())
                        SelectManager.ReqSelection(null);
                    GetTree().SetInputAsHandled();
                }
                break;
            // Add Move or Return to Idle
            case State.AddingNodes:
                if (@event.IsActionReleased("ui_accept"))
                {
                    if (_moveType == MoveType.March)
                    {
                        AddMoveNode(_ghost.GlobalPosition, Vector2.Right.Rotated(_ghost.GlobalRotation), false, Enumerable.Empty<String>());
                    }
                    else if (_moveType == MoveType.Reposition)
                    {
                        AddMoveNode(_ghost.GlobalPosition, Vector2.Right.Rotated(_ghost.GlobalRotation), false, new List<String> {"reposition"});
                    }
                    else if (_moveType == MoveType.Wheel)
                    {
                        AddMoveNode(_ghost.GlobalPosition, Vector2.Right.Rotated(_ghost.GlobalRotation), true, new List<String> {"wheel"});
                    }
                    GetTree().SetInputAsHandled();
                }
                else if (@event.IsActionReleased("ui_cancel"))
                {
                    ChangeState(State.Idle);
                    GetTree().SetInputAsHandled();
                }
                break;
            // Return to idle once done, adj happens in _process()
            case State.AdjustingNode:
                if (AdjustingNode != null && (@event.IsActionReleased("ui_accept") || @event.IsActionReleased("ui_cancel")))
                {
                    if (_moveType == MoveType.March)
                    {
                        AdjustingNode.SetAsLine(GetNodeStartGpos(AdjustingNode));
                    }
                    else if (_moveType == MoveType.Reposition)
                    {
                        AdjustingNode.SetAsLine(GetNodeStartGpos(AdjustingNode));
                        AdjustingNode.add_annotation("reposition");
                    }
                    else if (_moveType == MoveType.Wheel)
                    {
                        var a = new Trig.Arc2(GetNodeStartGpos(AdjustingNode), GetNodeStartGrot(AdjustingNode), AdjustingNode.GlobalPosition);
                        AdjustingNode.SetAsArc(a);
                        AdjustingNode.add_annotation("wheel");
                    }
                    else
                    {
                        // Ignore if invalid move
                        break;
                    }
                    AdjustingNode.Path.Modulate = IsSelected ? colPathSelected : colPathNotSelected;
                    ChangeState(State.Idle);
                    GetTree().SetInputAsHandled();
                }
                break;
            // Rotating is handled within OnMarkerDrag
            case State.RotatingNode:
            default:
                break;
        }
    }

    // TODO
    public Boolean OverlapsArea(Area2D area) => _end_marker.OverlapsArea(area);

    private void OnClickStart(MouseButton button)
    {
        // Select if not selected
        if (_state == State.NotSelected && button == MouseButton.Left && SelectManager.IsSelectionAllowed())
        {
            SelectManager.ReqSelection(_selectItem);
        }
        // Reset all nodes if idle and clicked
        else if (_state == State.Idle && button == MouseButton.Left)
        {
            ResetMoveNodes();
            ChangeState(State.AddingNodes);
        }
    }

    private void OnClickEnd(MouseButton button)
    {
        // Select if not selected
        if (_state == State.NotSelected && button == MouseButton.Left && SelectManager.IsSelectionAllowed())
        {
            SelectManager.ReqSelection(_selectItem);
            GetTree().SetInputAsHandled();
        }
        // Start adding nodes if idle
        else if (_state == State.Idle && button == MouseButton.Left)
        {
            ChangeState(State.AddingNodes);
            GetTree().SetInputAsHandled();
        }
        // Move end marker if click on, undo last
        else if (_state == State.AddingNodes && button == MouseButton.Left)
        {
            RmLastMoveNode();
            GetTree().SetInputAsHandled();
        }
    }

    private void OnClickChild(PositionNode node, MouseButton button)
    {
        if (_state == State.NotSelected && button == MouseButton.Left && SelectManager.IsSelectionAllowed())
        {
            SelectManager.ReqSelection(_selectItem);
            GetTree().SetInputAsHandled();
        }
        else if (_state == State.Idle && button == MouseButton.Left)
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

        if (marker.is_mouse_hovering)
        {
            GD.Print($"Mouse in {marker.Name}");
        }
        else
        {
            GD.Print($"Mouse out {marker.Name}");
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

    private void OnMarkerDrag(int index, Vector2 start, Vector2 end, MouseButton button, Boolean stopping, MouseArea2d marker)
    {
        if (MoveNodeCount < MoveNodeLimit && (_state == State.Idle || _state == State.AddingNodes))
        {
            ChangeState(State.RotatingNode);
        }
        else if (_state == State.RotatingNode)
        {
            if (!stopping)
            {
                if (_lastDragInd != index)
                {
                    GD.Print($"Dragging {marker.Name} with ind {index}");
                    _lastDragInd = index;
                }

                _ghost.GlobalPosition = marker.GlobalPosition;
                _ghost.GlobalRotation = (end - start).Angle();
            }
            else
            {
                GD.Print($"Stopped dragging {marker.Name} with ind {index}");
                AddMoveNode(marker.GlobalPosition, end - start, false, new List<String>() { "rotation" });
                ChangeState(State.Idle);
            }
        }
        else
        {
            // Do nothing
        }
    }

    private bool MoveNode(Node2D node, Vector2 gpos, float grot, Vector2 mpos)
    {
        var dir = Vector2.Right.Rotated(grot);
        var quarter = Trig.GetQuarter(gpos, dir, mpos);
        var side = Trig.GetSide(gpos, dir, mpos, 136f, 65f);
        _moveType = MoveType.None;
        _move_prev.Modulate = colPathAdding;
        Boolean canMove  = MoveNodeCount < MoveNodeLimit || _state == State.AdjustingNode;
        if (canMove && quarter == Trig.Quarter.front && side != Trig.Side.inside && Trig.DistToLine(new Trig.Ray2(gpos, dir), mpos) > 20f)
        {
            try
            {
                var arc = new Trig.Arc2(gpos, grot, mpos);
                arc = (arc.Length <= MaxFwdDistance) ? arc : new Trig.Arc2(arc, MaxFwdDistance);
                _move_prev.Points = Trig.SampleArc(arc, 20).Select(s => ToLocal(s)).ToArray();
                node.GlobalPosition = arc.End;
                node.GlobalRotation = arc.EndDir.Angle();
                _moveType = MoveType.Wheel;
            }
            catch
            {
                GD.Print($"Failed to make arc with {gpos} {grot} {mpos}");
            }
        }
        else if (canMove && quarter == Trig.Quarter.front && side != Trig.Side.inside)
        {
            var endGpos = Trig.NearestPointOnLine(new Trig.Ray2(gpos, dir), mpos);
            endGpos = (endGpos.DistanceTo(gpos) <= MaxFwdDistance) ? endGpos : dir * MaxFwdDistance + gpos;
            _move_prev.Points = new Vector2[] { ToLocal(gpos), ToLocal(endGpos) };
            node.GlobalPosition = endGpos;
            node.GlobalRotation = grot;
            _moveType = MoveType.March;
        }
        else if (canMove && (side == Trig.Side.left || side == Trig.Side.right))
        {
            var rdir = (side == Trig.Side.left) ? dir.Rotated(-Mathf.Pi/2f) : dir.Rotated(Mathf.Pi/2f);
            var endGpos = Trig.NearestPointOnLine(new Trig.Ray2(gpos, rdir), mpos);
            endGpos = (endGpos.DistanceTo(gpos) <= MaxSideDistance) ? endGpos :  rdir * MaxSideDistance + gpos;
            _move_prev.Points = new Vector2[] { ToLocal(gpos), ToLocal(endGpos) };
            node.GlobalPosition = endGpos;
            node.GlobalRotation = grot;
            _moveType = MoveType.Reposition;
        }
        else if (canMove && side == Trig.Side.back)
        {
            var endGpos = Trig.NearestPointOnLine(new Trig.Ray2(gpos, dir), mpos);
            endGpos = (endGpos.DistanceTo(gpos) <= MaxRvrDistance) ? endGpos : dir.Rotated(Mathf.Pi) * MaxRvrDistance + gpos;
            _move_prev.Points = new Vector2[] { ToLocal(gpos), ToLocal(endGpos) };
            node.GlobalPosition = endGpos;
            node.GlobalRotation = grot;
            _moveType = MoveType.Reposition;
        }
        else
        {
            return false;
        }

        return true;
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
                break;
            case State.AddingNodes:
                _move_prev.Points = new Vector2[0];
                _ghost.Hide();
                _moveType = MoveType.None;
                break;
            case State.RotatingNode:
                _ghost.Hide();
                break;
            case State.AdjustingNode:
                _move_prev.Points = new Vector2[0];
                _ghost.Hide();
                AdjustingNode?.Sprite.Show();
                for(var node = _nodeHead; node != null; node = node.Next)
                    node.Enable();
                _nodeTail?.Disable();
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
                _moveType = MoveType.None;
                break;
            case State.RotatingNode:
                GD.Print("To State RotatingNode " + Name);
                _ghost.Show();
                break;
            case State.AdjustingNode:
                GD.Print("To State AdjustingNode " + Name);
                for(var node = _nodeHead; node != null; node = node.Next)
                    node.Disable();
                AdjustingNode.Enable();
                AdjustingNode.clear_path();
                AdjustingNode.clear_annotations();
                AdjustingNode.Sprite.Hide();
                _ghost.Show();
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
        if (MoveNodeCount >= MoveNodeLimit)
        {
            return null;
        }

        GD.Print("Add move");
        var inst = _posNodeScene.Instance() as PositionNode;
        inst.Connect(nameof(PositionNode.clicked_on_hover), this, nameof(OnClickChild));
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
            _nodeTail.SetAsLine(start);
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

        MoveNodeCount++;
        GD.Print(MoveNodeCount);
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
            MoveNodeCount--;
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
        MoveNodeCount = 0;

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

    public Vector2 GetTailGpos() => _nodeTail != null ? _nodeTail.GlobalPosition : GlobalPosition;
    public float GetTailGrot() => _nodeTail != null ? _nodeTail.GlobalRotation : GlobalRotation;
}
