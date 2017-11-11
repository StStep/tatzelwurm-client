# Handles movement command segments

extends Node2D

const C_NOT_HIGHLIGHTED = Color('ffffff') # White
const C_HIGHLIGHT = Color('b6ff00') # Green-Yellow

onready var marker = get_node('Marker')

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
	marker.connect('state_changed', self, '_render_marker_highlight')

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

# Not visible when not-selected, so logic works out
func _render_marker_highlight():
	if not unit.is_busy() and marker.is_highlighted:
		marker.get_node("Sprite").modulate = C_HIGHLIGHT
	else:
		marker.get_node("Sprite").modulate = C_NOT_HIGHLIGHTED

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

func handle_input(ev):
	if marker.is_highlighted and not unit.is_busy():
		unit.mv_adj = self
		unit.handle_input(ev)

