# Handle overall game-logic

extends Node

signal miss_click

var selUnit = null setget _selectUnit, _retSelUnit
# >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
func _selectUnit(value):
	# Skip selecting if already selected
	if value == selUnit:
		return

	# Skip selecting if failed to deselect
	if not DeselectUnit():
		return

	if value != null and value.has_method('Select'):
		value.Select()

	selUnit = value

func _retSelUnit():
	return selUnit
# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


func _ready():
	set_process_unhandled_input(true)

# Catch 'missed' clicks
func  _unhandled_input(event):
	if event is InputEventMouseButton and event.button_index == BUTTON_LEFT and event.is_pressed():
		emit_signal("miss_click", BUTTON_LEFT)
		get_tree().set_input_as_handled()
	elif event is InputEventMouseButton and event.button_index == BUTTON_RIGHT and event.is_pressed():
		emit_signal("miss_click", BUTTON_RIGHT)
		get_tree().set_input_as_handled()
	else:
		pass

# Deselect the selected unit
# Return success
func DeselectUnit():
	var ret = true
	if selUnit != null and selUnit.has_method('Deselect'):
		ret = selUnit.Deselect()
	if ret:
		selUnit = null
	return ret
