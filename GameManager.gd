# Handle overall game-logic

extends Node

var selUnit

func ReqSelection(value):
	if value == null or value == selUnit:
		return

	# Check for highlight i/f
	if not value.has_method("Select") or not value.has_method("Deselect") or not value.has_method("HandleInput") \
			or not value.has_method("IsBusy"):
		print("Warning: Selected unit missing Select functions")
		return

	ReqDeselection()
	selUnit = value
	value.Select()

func ReqDeselection():
	if selUnit == null:
		return
	selUnit.Deselect()
	selUnit = null

var highUnits = []

func ReqHighlight(value):
	if value == null or value in highUnits:
		return

	# Check for highlight i/f
	if not value.has_method("Highlight") or not value.has_method("Unhighlight") or not value.has_method("HandleInput"):
		print("Warning: Hightlighted unit missing hightlight functions")
		return

	# Only highlight others if selected is not busy
	if selUnit != null and selUnit.IsBusy():
		return

	highUnits.append(value)
	value.Highlight()

func ReqUnhighlight(value):
	if value == null or not value in highUnits:
		return
	highUnits.erase(value)
	value.Unhighlight()

func _ready():
	set_process_input(true)

func _input(ev):
	# Pass input through, stop once handled
	if selUnit != null and selUnit.HandleInput(ev):
		return

	for u in highUnits:
		if u.HandleInput(ev):
			return
