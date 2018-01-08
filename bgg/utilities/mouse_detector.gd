# Handles mouse detection over Area2Ds

extends Area2D

signal mouse_hover_changed()
signal event_while_hovering_occured(event)

var is_mouse_hovering = false setget ,_is_mouse_hovering_get
func _is_mouse_hovering_get():
	return is_mouse_hovering

func _ready():
	set_process_input(false)
	connect('mouse_entered', self, '_on_mouse_enter')
	connect('mouse_exited', self, '_on_mouse_exit')
	connect('visibility_changed', self, '_on_visibility_change')

func _input(ev):
	emit_signal("event_while_hovering_occured", ev)

func _on_mouse_enter():
	mark_as_hovering()

func _on_mouse_exit():
	mark_as_not_hovering()

func _on_visibility_change():
	if not visible and is_mouse_hovering:
		mark_as_not_hovering()

# For when markers are manually moved
func mark_as_hovering():
	is_mouse_hovering = true
	set_process_input(is_mouse_hovering)
	emit_signal("mouse_hover_changed")

# For when markers are manually moved
func mark_as_not_hovering():
	is_mouse_hovering = false
	set_process_input(is_mouse_hovering)
	emit_signal("mouse_hover_changed")
