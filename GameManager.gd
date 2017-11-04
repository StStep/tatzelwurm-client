# Handle overall game-logic

extends Node

var selected_unit = null
var highlighted_units = []

func _ready():
	set_process_input(true)

func _input(ev):
	# Pass input through, stop once handled
	if selected_unit != null and selected_unit.handle_input(ev):
		return

	for u in highlighted_units:
		if u.handle_input(ev):
			return

func req_selection(value):
	if value == null or value == selected_unit:
		return

	# Check for highlight i/f
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

func req_highlight(value):
	if value == null or value in highlighted_units:
		return

	# Check for highlight i/f
	if not value.has_method("highlight") \
			or not value.has_method("unhighlight") \
			or not value.has_method("handle_input"):
		print("Warning: Hightlighted unit missing hightlight functions")
		return

	# Only highlight others if selected is not busy
	if selected_unit != null and selected_unit.is_busy():
		return

	highlighted_units.append(value)
	value.highlight()

func req_unhighlight(value):
	if value == null or not value in highlighted_units:
		return
	highlighted_units.erase(value)
	value.unhighlight()
