extends Node2D

# Global start point
var start = Vector2() setget _setStart, _getStart
# Global end point
var end = Vector2() setget _setEnd, _getEnd
# Prev node in list
var previous = null
# Next node in list
var next = null
# The move this command represents 
var move = Vector2(0, 0)

onready var path = get_node('Path')

func _ready():
	get_node('Marker').connect('single_click', self, '_on_marker_click')

func _setStart(value):
	pass

func _getStart():
	return previous.end

func _setEnd(gpos):
	move = gpos - previous.end
	global_position = gpos
	Update()

func _getEnd():
	return to_global(Vector2(0,0))

func _on_marker_click(button):
	print("Click")

func Enable():
	get_node('Marker').Enable()
	get_node('Marker').show()

func Disable():
	get_node('Marker').Disable()
	get_node('Marker').hide()

func Update():
	global_position = previous.end + move
	get_node("Path").points = PoolVector2Array([to_local(previous.end), Vector2(0,0)]) # Takes local pos
	if next: next.Update()
	
