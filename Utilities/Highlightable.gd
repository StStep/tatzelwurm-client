# Handles highlighting of Area2Ds
#
# Expects Selectable Parent
# Expects children:
# 	Shape - CollisionShape2D

extends Area2D

onready var par = get_parent()
onready var gm = get_node('/root/GameManager')
var is_highlighted = false

signal highlighted

func _ready():
	connect('mouse_entered', self, '_on_mouse_enter')
	connect('mouse_exited', self, '_on_mouse_exit')

func _on_mouse_enter():
	gm.req_highlight(self)

func _on_mouse_exit():
	gm.req_unhighlight(self)

# Highlightable Interface
# >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
func handle_input(ev):
	par.handle_input(ev)

func highlight():
	is_highlighted = true
	emit_signal("highlighted")

func unhighlight():
	is_highlighted = false
	emit_signal("highlighted")
# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
