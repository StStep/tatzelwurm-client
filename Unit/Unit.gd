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


var IsSelected = false
var state = STATE.None
var marker_color = C_IDLE
onready var ghost = get_node('Ghost')
onready var marker_sprite = get_node('Marker/Sprite')
onready var movePrev = get_node('MovePreview')
var movCmd = load("res://Unit/MoveCmd.tscn")

# Tail of move cmd list
var mvTail = self
# For moveCmds to reference
onready var end = global_position
# First movecmd
var next = null


func _ready():
	set_process(true)
	get_node('Marker').connect('single_click', self, '_on_marker_click')
	get_node('Marker').connect('mouse_entered', self, '_on_marker_enter')
	get_node('Marker').connect('mouse_exited', self, '_on_marker_exit')
	get_node('/root/GameManager').connect('miss_click', self, '_on_miss_click')


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
		marker_sprite.set_modulate(C_HIGHLIGHT)

func _on_marker_exit():
	marker_sprite.set_modulate(marker_color)

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
	marker_sprite.set_modulate(C_SELECTED)
	marker_color = C_SELECTED
	var node = next
	while node:
		node.Enable()
		node = node.next
	print('Selected ' + get_name())

func Deselect():
	IsSelected = false
	marker_sprite.set_modulate(C_IDLE)
	marker_color = C_IDLE
	var node = next
	while node:
		node.Disable()
		node = node.next
	print('Deselected ' + get_name())

func _addMoveSeg(gpos):
	print('Add move')
	var inst = movCmd.instance()
	add_child(inst)
	inst.previous = mvTail
	mvTail.next = inst
	mvTail = inst
	inst.end = gpos
