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
	mark_as_highlighted()

func _on_mouse_exit():
	mark_as_unhighlighted()

# For when markers are manually moved
func mark_as_highlighted():
	gm.req_highlight(self)

# For when markers are manually moved
func mark_as_unhighlighted():
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
