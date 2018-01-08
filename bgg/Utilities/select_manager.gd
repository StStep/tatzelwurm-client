# Handle overall game-logic

extends Node

var selected_unit = null

func _ready():
	set_process_input(true)

func _input(ev):
	# Pass input through, stop once handled
	if selected_unit != null and selected_unit.handle_input(ev):
		get_tree().set_input_as_handled()
		return

func is_selection_allowed():
	if selected_unit != null and selected_unit.is_busy():
		return false
	else:
		return true

func req_selection(value):
	if value == null or value == selected_unit:
		return

	# Check for select_item i/f
	if not value.has_method("select") \
			or not value.has_method("deselect") \
			or not value.has_method("handle_input") \
			or not value.has_method("is_busy"):
		print("Warning: Selected unit missing select functions")
		return

	req_deselection()
	selected_unit = value
	value.select()

func req_deselection():
	if selected_unit == null:
		return
	selected_unit.deselect()
	selected_unit = null
