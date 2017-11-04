# Handled individual unit logic
#
# Has Ghost and Marker children
# Handles click connections

extends Node2D

enum STATE {
	Not_Selected,
	Idle,
	Add_Mv_Single,
	Add_mv_Cont,
}

const C_IDLE = Color('ffffff') # White
const C_SELECTED = Color('f6ff00') # Yellow
const C_HIGHLIGHT = Color('b6ff00') # Green-Yellow
const C_PATH_IDLE = Color('66ff68') # Pastel-Green
const C_PATH_SELECTED = Color('16ab19') # Green
const C_PATH_HIGHLIGHT = Color('b6ff00') # Green-Yellow


var state = STATE.Not_Selected
var isHighlighted = false
var marker_color = C_IDLE
var path_color = C_PATH_IDLE
onready var gm = get_node('/root/GameManager')
onready var ghost = get_node('Ghost')
onready var start_marker_sprite = get_node('StartMarker/Sprite')
onready var end_marker_sprite = get_node('EndMarker/Sprite')
onready var movePrev = get_node('MovePreview')
onready var endMarker = get_node("EndMarker")
var movCmd = load("res://Unit/MoveCmd.tscn")

# Tail of move cmd list
var mvTail = self
# For moveCmds to reference
onready var end = global_position
# First movecmd
var next = null

func _ready():
	set_process(true)
	get_node('StartMarker').connect('mouse_entered', self, '_on_marker_enter')
	get_node('StartMarker').connect('mouse_exited', self, '_on_marker_exit')
	get_node('EndMarker').connect('mouse_entered', self, '_on_marker_enter')
	get_node('EndMarker').connect('mouse_exited', self, '_on_marker_exit')
	endMarker.Disable()
	endMarker.hide()

func _process(delta):
	if state == STATE.Add_mv_Cont:
		var mpos = get_viewport().get_mouse_position()
		ghost.global_position = mpos
		movePrev.points = PoolVector2Array([to_local(mvTail.end), to_local(mpos)])

func HandleInput(ev):
	var ret = false
	match state:
		# Select if hightlighted ----------------------
		STATE.Not_Selected:
			if isHighlighted and ev.is_action_pressed("ui_accept"):
				gm.ReqSelection(self)
				ret = true
		# Start adding moves if hightlighted or deselect ----------------------
		STATE.Idle:
			ret = true
			if isHighlighted and ev.is_action_pressed("ui_accept"):
				ChangeState(STATE.Add_mv_Cont)
			elif ev.is_action_pressed("ui_accept") or ev.is_action_pressed("ui_cancel"):
				gm.ReqDeselection()
			else:
				ret = false
		# Add Move or Return to Idle ----------
		STATE.Add_mv_Cont:
			ret = true
			if ev.is_action_pressed("ui_accept"):
				_addMoveSeg(ghost.global_position)
			elif ev.is_action_pressed("ui_cancel"):
				ChangeState(STATE.Idle)
			else:
				ret = false
		_:
			pass
	return ret

func _on_marker_click(button):
	match state:
		STATE.Not_Selected:
			if button == BUTTON_LEFT:
				gm.selUnit = self
		STATE.Idle:
			ChangeState(STATE.Add_mv_Cont)

func _on_marker_enter():
	gm.ReqHighlight(self)

func _on_marker_exit():
	gm.ReqUnhighlight(self)

func ChangeState(s):
	if s == state:
		return

	# Prev State
	match state:
		STATE.Not_Selected:
			pass
		STATE.Idle:
			pass
		STATE.Add_mv_Cont:
			movePrev.points = PoolVector2Array()
			ghost.hide()

	# New State
	match s:
		STATE.Not_Selected:
			_deselect()
		STATE.Idle:
			_select()
		STATE.Add_mv_Cont:
			ghost.show()

	state = s

# Determines if this unit can be deselected
func IsBusy():
	if state == STATE.Not_Selected or state == STATE.Idle:
		return false
	else:
		return true

func Select():
	ChangeState(STATE.Idle)

func _select():
	start_marker_sprite.modulate = C_SELECTED
	end_marker_sprite.modulate = C_SELECTED
	marker_color = C_SELECTED
	path_color = C_PATH_SELECTED
	var node = next
	while node:
		node.Enable()
		node.path.modulate = C_PATH_SELECTED
		node = node.next
	# Always hide last node under end marker
	if mvTail != self:
		mvTail.Disable()
	print('Selected ' + get_name())

func Deselect():
	ChangeState(STATE.Not_Selected)

func _deselect():
	start_marker_sprite.modulate = C_IDLE
	end_marker_sprite.modulate = C_IDLE
	marker_color = C_IDLE
	path_color = C_PATH_IDLE
	var node = next
	while node:
		node.Disable()
		node.path.modulate = C_PATH_IDLE
		node = node.next
	print('Deselected ' + get_name())

func Highlight():
	isHighlighted = true
	start_marker_sprite.modulate = C_HIGHLIGHT
	end_marker_sprite.modulate = C_HIGHLIGHT
	var node = next
	while node:
		node.path.modulate = C_PATH_HIGHLIGHT
		node = node.next

func Unhighlight():
	isHighlighted = false
	start_marker_sprite.modulate = marker_color
	end_marker_sprite.modulate = marker_color
	var node = next
	while node:
		node.path.modulate = path_color
		node = node.next

func _addMoveSeg(gpos):
	print('Add move')
	var inst = movCmd.instance()
	add_child(inst)

	# Disable point under end marker, enable prev hidden
	if mvTail != self:
		mvTail.Enable()
	inst.Disable()
	inst.previous = mvTail
	mvTail.next = inst
	mvTail = inst
	inst.end = gpos
	inst.path.modulate = C_PATH_SELECTED

	# Move up ghost marker
	endMarker.Enable()
	endMarker.show()
	endMarker.global_position = mvTail.end
