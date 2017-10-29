# Handled individual unit logic
#
# Has Ghost and Marker children
# Handles click connections

extends Node2D

enum STATE {
  None,
  Moving
}

const C_IDLE = Color('ffffff') # White
const C_SELECTED = Color('f6ff00') # Yellow
const C_HIGHLIGHT = Color('b6ff00') # Green-Yellow
const C_PATH_IDLE = Color('66ff68') # Pastel-Green
const C_PATH_SELECTED = Color('16ab19') # Green
const C_PATH_HIGHLIGHT = Color('b6ff00') # Green-Yellow


var IsSelected = false
var state = STATE.None
var marker_color = C_IDLE
var path_color = C_PATH_IDLE
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
	get_node('StartMarker').connect('single_click', self, '_on_marker_click')
	get_node('EndMarker').connect('single_click', self, '_on_marker_click')
	get_node('StartMarker').connect('mouse_entered', self, '_on_marker_enter')
	get_node('StartMarker').connect('mouse_exited', self, '_on_marker_exit')
	get_node('EndMarker').connect('mouse_entered', self, '_on_marker_enter')
	get_node('EndMarker').connect('mouse_exited', self, '_on_marker_exit')
	get_node('/root/GameManager').connect('miss_click', self, '_on_miss_click')
	endMarker.Disable()
	endMarker.hide()


func _process(delta):
	if state == STATE.Moving:
		var mpos = get_viewport().get_mouse_position()
		ghost.global_position = mpos
		movePrev.points = PoolVector2Array([to_local(mvTail.end), to_local(mpos)])


func _on_miss_click(button):
	match state:
		# Deselect ----------------------
		STATE.None:
			match button:
				BUTTON_LEFT, BUTTON_RIGHT:
					if IsSelected: get_node('/root/GameManager').TryDeselectUnit()
		# Add Move or Deselect ----------
		STATE.Moving:
			match button:
				BUTTON_LEFT:
					_addMoveSeg(ghost.global_position)
				BUTTON_RIGHT:
					ChangeState(STATE.None)


func _on_marker_click(button):
	match state:
		# Select or start moving if selected
		STATE.None:
			if IsSelected:
				ChangeState(STATE.Moving)
			else:
				if button == BUTTON_LEFT:
					get_node('/root/GameManager').selUnit = self
		STATE.Moving:
			pass

func _on_marker_enter():
	if not get_node('/root/GameManager').IsBusy():
		start_marker_sprite.modulate = C_HIGHLIGHT
		end_marker_sprite.modulate = C_HIGHLIGHT
		var node = next
		while node:
			node.path.modulate = C_PATH_HIGHLIGHT
			node = node.next

func _on_marker_exit():
	start_marker_sprite.modulate = marker_color
	end_marker_sprite.modulate = marker_color
	var node = next
	while node:
		node.path.modulate = path_color
		node = node.next

func ChangeState(s):
	if s == state:
		return

	# Prev State
	match state:
		STATE.None:
			pass
		STATE.Moving:
			movePrev.points = PoolVector2Array()
			ghost.hide()

	# New State
	match s:
		STATE.None:
			pass
		STATE.Moving:
			ghost.show()

	state = s

# Determines if this unit can be deselected
func IsBusy():
	if state == STATE.None:
		return false
	else:
		return true

func Select():
	IsSelected = true
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
	IsSelected = false
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
