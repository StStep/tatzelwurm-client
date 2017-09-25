# Handled individual unit logic
#
# Has Ghost and Marker children
# Handles click connections

extends Node2D

enum STATE {
  None,
  Moving
}

var IsSelected = false
var state = STATE.None
onready var ghost = get_node('Ghost')


func _ready():
	set_process(true)
	get_node('Marker').connect('single_click', self, '_on_marker_click')
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
	get_node('Marker/Sprite').set_modulate(Color('f6ff00')) # Yellow
	print('Selected ' + get_name())


func Deselect():
	if state == STATE.None:
		IsSelected = false
		get_node('Marker/Sprite').set_modulate('ffffff') # White
		print('Deselected ' + get_name())
		return true
	else:
		return false
