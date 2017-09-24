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
	get_node('/root/game_manager').connect('miss_click', self, '_on_miss_click')


func _process(delta):
	if state == STATE.Moving:
		ghost.set_global_pos(get_viewport().get_mouse_pos())


func _on_miss_click(button):
	if button == BUTTON_LEFT or button == BUTTON_RIGHT:
		if IsSelected:
			get_node('/root/game_manager').DeselectUnit()
	else:
		pass


func _on_marker_click(button):
	if button == BUTTON_LEFT:
		 get_node('/root/game_manager').selUnit = self
	else:
		pass


func Select():
	IsSelected = true
	get_node('Marker/Sprite').set_modulate(Color('f6ff00')) # Yellow
	state = STATE.Moving
	ghost.show()
	print('Selected ' + get_name())


func Deselect():
	IsSelected = false
	state = STATE.None
	ghost.hide()
	get_node('Marker/Sprite').set_modulate('ffffff') # White
	print('Deselected ' + get_name())
	return true
