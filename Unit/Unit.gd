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
	# Deselect
	if state == STATE.None:
		if button == BUTTON_LEFT or button == BUTTON_RIGHT:
			if IsSelected:
				get_node('/root/GameManager').DeselectUnit()
		else:
			pass
	# Add Move or Deselect
	elif state == STATE.Moving:
		if button == BUTTON_LEFT:
			print('Add move')
		elif button == BUTTON_RIGHT:
			ChangeState(STATE.None)
		else:
			pass
	else:
		pass

# TODO Use Match?
func _on_marker_click(button):
	# Select or start moving if selected
	if state == STATE.None:
		if IsSelected:
			ChangeState(STATE.Moving)
		else:
			if button == BUTTON_LEFT:
				get_node('/root/GameManager').selUnit = self
	elif state == STATE.Moving:
		pass
	else:
		pass


func ChangeState(s):
	if s == state:
		return

	# Prev State
	if state == STATE.None:
		pass
	elif state == STATE.Moving:
		ghost.hide()
	else:
		pass

	# New State
	if s == STATE.None:
		pass
	elif s == STATE.Moving:
		ghost.show()
	else:
		pass

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
