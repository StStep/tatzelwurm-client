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


func _ready():
	set_process(true)
	get_node('Marker').connect('single_click', self, '_on_marker_click')
	get_node('Marker').connect('mouse_entered', self, '_on_marker_enter')
	get_node('Marker').connect('mouse_exited', self, '_on_marker_exit')
	get_node('/root/GameManager').connect('miss_click', self, '_on_miss_click')


func _process(delta):
	if state == STATE.Moving:
		ghost.global_position = get_viewport().get_mouse_position()


func _on_miss_click(button):
	match state:
		# Deselect ----------------------
		STATE.None:
			match button:
				BUTTON_LEFT, BUTTON_RIGHT:
					if IsSelected: get_node('/root/GameManager').DeselectUnit()
		# Add Move or Deselect ----------
		STATE.Moving:
			match button:
				BUTTON_LEFT:
					print('Add move')
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
			ghost.hide()

	# New State
	match s:
		STATE.None:
			pass
		STATE.Moving:
			ghost.show()

	state = s


func Select():
	IsSelected = true
	marker_sprite.set_modulate(C_SELECTED)
	marker_color = C_SELECTED
	print('Selected ' + get_name())


func Deselect():
	if state == STATE.None:
		IsSelected = false
		marker_sprite.set_modulate(C_IDLE)
		marker_color = C_IDLE
		print('Deselected ' + get_name())
		return true
	else:
		return false
