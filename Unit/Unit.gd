# Handled individual unit logic
#
# Has Ghost and Marker children

extends Node2D

enum STATE {
	Not_Selected,
	Idle,
	Add_Mv_Single,
	Add_mv_Cont,
}

const C_NOT_SELECTED = Color('ffffff') # White
const C_SELECTED = Color('f6ff00') # Yellow
const C_HIGHLIGHT = Color('b6ff00') # Green-Yellow
const C_PATH_NOT_SELECTED = Color('66ff68') # Pastel-Green
const C_PATH_SELECTED = Color('16ab19') # Green
const C_PATH_HIGHLIGHT = Color('b6ff00') # Green-Yellow

var state = STATE.Not_Selected
var marker_color = C_NOT_SELECTED
var path_color = C_PATH_NOT_SELECTED
onready var gm = get_node('/root/GameManager')
onready var ghost = get_node('Ghost')
onready var start_marker_sprite = get_node('StartMarker/Sprite')
onready var end_marker_sprite = get_node('EndMarker/Sprite')
onready var move_prev = get_node('MovePreview')
onready var start_marker = get_node("StartMarker")
onready var end_marker = get_node("EndMarker")
var mov_cmd = load("res://Unit/MoveCmd.tscn")

# Tail of move cmd list
var mv_tail = null
# First MoveCmd
var mv_head = null

func _ready():
	set_process(true)
	start_marker.connect('state_changed', self, '_render_marker_highlight', [start_marker])
	end_marker.connect('state_changed', self, '_render_marker_highlight', [end_marker])
	end_marker.hide()

func _process(delta):
	if state == STATE.Add_mv_Cont:
		var mpos = get_viewport().get_mouse_position()
		ghost.global_position = mpos
		var end = mv_tail.end if mv_tail else global_position
		move_prev.points = PoolVector2Array([to_local(end), to_local(mpos)])

func _highlight():
	start_marker_sprite.modulate = C_HIGHLIGHT
	end_marker_sprite.modulate = C_HIGHLIGHT
	var node = mv_head
	while node:
		node.path.modulate = C_PATH_HIGHLIGHT
		node = node.next

func _unhighlight():
	start_marker_sprite.modulate = marker_color
	end_marker_sprite.modulate = marker_color
	var node = mv_head
	while node:
		node.path.modulate = path_color
		node = node.next

func _render_marker_highlight(marker):
	# Highlight everything if not yet selected
	if state == STATE.Not_Selected and marker.is_highlighted \
			and gm.is_selection_allowed():
		_highlight()
	# If busy or not selected, don't highlight anything
	elif state == STATE.Not_Selected or is_busy():
		_unhighlight()
	# Else only highlight self
	elif marker.is_highlighted:
		marker.get_node("Sprite").modulate = C_HIGHLIGHT
	else:
		marker.get_node("Sprite").modulate = marker_color

func _on_select():
	start_marker_sprite.modulate = C_SELECTED
	end_marker_sprite.modulate = C_SELECTED
	marker_color = C_SELECTED
	path_color = C_PATH_SELECTED
	var node = mv_head
	while node:
		node.enable()
		node.path.modulate = C_PATH_SELECTED
		node = node.next
	# Always hide last node under end marker
	if mv_tail:
		mv_tail.disable()
	print('Selected ' + get_name())

func _on_deselect():
	start_marker_sprite.modulate = C_NOT_SELECTED
	end_marker_sprite.modulate = C_NOT_SELECTED
	marker_color = C_NOT_SELECTED
	path_color = C_PATH_NOT_SELECTED
	var node = mv_head
	while node:
		node.disable()
		node.path.modulate = C_PATH_NOT_SELECTED
		node = node.next
	print('Deselected ' + get_name())

func _on_mv_reset():
	mv_tail = null
	if mv_head:
		mv_head.erase()
		mv_head = null
	end_marker.hide()

func _change_state(s):
	if s == state:
		return

	# Prev State
	match state:
		STATE.Add_mv_Cont:
			move_prev.points = PoolVector2Array()
			ghost.hide()
		_:
			pass

	# New State
	match s:
		STATE.Not_Selected:
			_on_deselect()
		STATE.Idle:
			_on_select()
		STATE.Add_mv_Cont:
			ghost.show()
		_:
			pass

	state = s

	# Refresh marker colors
	gm.refresh_highlighted_render()

func _add_move_seg(gpos):
	print('Add move')
	var inst = mov_cmd.instance()
	inst.unit = self
	add_child(inst)

	# disable point under end marker, enable prev hidden
	if not mv_head:
		mv_head = inst
	else:
		mv_tail.enable()
		mv_tail.next = inst
	inst.disable()
	inst.previous = mv_tail
	mv_tail = inst
	inst.end = gpos
	inst.path.modulate = C_PATH_SELECTED

	# Move up ghost marker
	end_marker.show()
	end_marker.global_position = mv_tail.end

# Determines if this unit can be deselected
func is_busy():
	if state == STATE.Not_Selected or state == STATE.Idle:
		return false
	else:
		return true

# Selectable Interface
# >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
func handle_input(ev):
	var ret = false
	match state:
		# select if hightlighted
		STATE.Not_Selected:
			if (start_marker.is_highlighted or end_marker.is_highlighted) \
					and ev.is_action_pressed("ui_accept") \
					and gm.is_selection_allowed():
				gm.req_selection(self)
				ret = true
		# Start adding moves if hightlighted or deselect
		STATE.Idle:
			ret = true
			if start_marker.is_highlighted and ev.is_action_pressed("ui_accept"):
				_on_mv_reset()
				_change_state(STATE.Add_mv_Cont)
			elif end_marker.is_highlighted and ev.is_action_pressed("ui_accept"):
				_change_state(STATE.Add_mv_Cont)
			elif (gm.highlighted_units.empty() and ev.is_action_pressed("ui_accept")) \
					or ev.is_action_pressed("ui_cancel"):
				gm.req_deselection()
			else:
				ret = false
		# Add Move or Return to Idle
		STATE.Add_mv_Cont:
			ret = true
			if ev.is_action_pressed("ui_accept"):
				_add_move_seg(ghost.global_position)
			elif ev.is_action_pressed("ui_cancel"):
				_change_state(STATE.Idle)
			else:
				ret = false
		_:
			pass
	return ret

func select():
	_change_state(STATE.Idle)

func deselect():
	_change_state(STATE.Not_Selected)
# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
