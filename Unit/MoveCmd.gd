# Handles movement command segments

extends Node2D

# Parent Unit
var unit
# Global start point
var start = Vector2() setget _set_start, _get_start
# Global end point
var end = Vector2() setget _set_end, _get_end
# Prev node in list
var previous = null
# Next node in list
var next = null
# The move this command represents
var move = Vector2(0, 0)

onready var path = get_node('Path')

func _ready():
	pass

func _set_start(value):
	pass

func _get_start():
	if not previous:
		return unit.global_position
	else:
		return previous.end

func _set_end(gpos):
	move = gpos - _get_start()
	global_position = gpos
	update()

func _get_end():
	return to_global(Vector2(0,0))

func enable():
	get_node('Marker').show()

func disable():
	get_node('Marker').hide()

func update():
	global_position = _get_start() + move
	get_node("Path").points = \
		PoolVector2Array([to_local(_get_start()), Vector2(0,0)])
	if next: next.update()

func erase():
	if next: next.erase()
	queue_free()

