# Handle selection logic, before requesting a selection, verify
# that a selection is allowed
# clear by requesting a selection of null

extends Node

var selected_item = null setget ,_selected_item_get
func _selected_item_get():
	return selected_item

func _ready():
	set_process_input(true)

func _input(ev):
	if selected_item != null:
		selected_item.accept_input(ev)

func is_selection_allowed():
	if selected_item != null and selected_item.is_busy:
		return false
	else:
		return true

func req_selection(value):
	if value == null:
		_clear_selection()
		return
	elif value == selected_item:
		return

	# Check for select_item i/f
	if not value.has_method("select") \
			or not value.has_method("deselect") \
			or not value.has_method("accept_input"):
		print("Warning: select_item missing functions")
		return

	_clear_selection()
	selected_item = value
	value.select()

func _clear_selection():
	if selected_item == null:
		return
	selected_item.deselect()
	selected_item = null
