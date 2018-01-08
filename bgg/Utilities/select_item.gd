# Allows for selection by select_manager

extends Node

signal selection_changed(is_selected)
signal item_event_occured(event)

# Set externally
var is_busy = false

# Set internally
var is_selected = false setget ,_is_selected_get
func _is_selected_get():
	return is_selected

func select():
	is_selected = true
	emit_signal("selection_changed", is_selected)

func deselect():
	is_selected = false
	emit_signal("selection_changed", is_selected)

func handle_input(ev):
	emit_signal("item_event_occured", ev)

func is_busy():
	return is_busy
