# Handles movement command segments

extends Node2D

const C_NOT_HIGHLIGHTED = Color('ffffff') # White
const C_HIGHLIGHT = Color('b6ff00') # Green-Yellow

onready var marker = get_node('Marker')
onready var path_area = get_node('PathArea')

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
onready var path_shape = get_node('PathArea/Shape')

func _ready():
	marker.connect('state_changed', self, '_render_marker_highlight')
	path_area.connect('state_changed', self, '_render_path_highlight')

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

func _render_marker_highlight():
	if unit.is_selected() and not unit.is_busy() and marker.is_highlighted:
		marker.get_node("Sprite").modulate = C_HIGHLIGHT
	else:
		marker.get_node("Sprite").modulate = C_NOT_HIGHLIGHTED

func _render_path_highlight():
	if unit.is_selected() and not unit.is_busy() and path_area.is_highlighted:
		unit.high_path = self

func enable():
	get_node('Marker').show()

func disable():
	get_node('Marker').hide()

func update():
	global_position = _get_start() + move
	var l_vec = to_local(_get_start())
	path.points = PoolVector2Array([l_vec, Vector2(0,0)])
	var shape = RectangleShape2D.new()
	shape.extents = Vector2(10, l_vec.length()/2)
	path_shape.set_shape(shape)
	path_shape.rotation = l_vec.angle() + PI/2
	path_shape.position = l_vec/2
	if next: next.update()

func erase():
	if next: next.erase()
	queue_free()

func closest_pnt_on_path(gpos):
	var pnt = to_local(gpos)
	var dir = to_local(_get_start()).normalized()
	var dot = pnt.dot(dir)
	return to_global(dir*dot)

func handle_input(ev):
	if marker.is_highlighted and not unit.is_busy():
		unit.mv_adj = self
		unit.handle_input(ev)

