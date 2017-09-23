extends Node

var selUnit = null setget _selectUnit, _retSelUnit

func _ready():
	set_process_unhandled_input(true)

# Catch undhandled clicks
func  _unhandled_input(event):
	if event.type == InputEvent.MOUSE_BUTTON and event.button_index == BUTTON_LEFT and event.is_pressed():
		get_node('/root/game_manager').DeselectUnit()
		get_tree().set_input_as_handled()

func DeselectUnit():
	if selUnit != null and selUnit.has_method('Deselect'):
		selUnit.Deselect()
	selUnit = null

func _selectUnit(value):
	if value == selUnit:
		return

	DeselectUnit()
	if value != null and value.has_method('Select'):
		value.Select()

	selUnit = value

func _retSelUnit():
	return selUnit