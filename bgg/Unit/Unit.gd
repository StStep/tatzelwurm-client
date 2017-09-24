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

func _process(delta):
	if state == STATE.Moving:
		ghost.set_global_pos(get_viewport().get_mouse_pos())

func Select():
	IsSelected = true
	get_node('Unit Marker/Sprite').set_modulate(Color('f6ff00')) # Yellow
	state = STATE.Moving
	ghost.show()
	print('Selected ' + get_name())

func Deselect():
	IsSelected = false
	state = STATE.None
	ghost.hide()
	get_node('Unit Marker/Sprite').set_modulate('ffffff') # White
	print('Deselected ' + get_name())
