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
	gm.ReqHighlight(self)

func _on_mouse_exit():
	gm.ReqUnhighlight(self)

# Highlightable Interface
# >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
func HandleInput(ev):
	par.HandleInput(ev)

func Highlight():
	is_highlighted = true
	emit_signal("highlighted")

func Unhighlight():
	is_highlighted = false
	emit_signal("highlighted")
# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
