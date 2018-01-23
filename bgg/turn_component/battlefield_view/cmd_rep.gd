# cmd_rep.gd
#
# This Scene renders a command in the battlefield view

extends Node

#### Constants

const Trig = preload('res://utilities/trig.gd')

#### Signals

# TODO

#### Variables

# TODO Make Private

onready var sprite_node = get_node('Sprite')
onready var reposition_node = get_node('Reposition')
onready var wheel_node = get_node('Wheel')
onready var rotation_node = get_node('Rotation')
onready var path_node = get_node('Path')

# Possible annotations that can be displayed
onready var _annotation = {
	'reposition' : reposition_node,
	'wheel' : wheel_node,
	'rotation' : rotation_node,
}

#### Private Function

# Node function, called once all children are ready
func _ready():
	clear_annotations()

#### Public Functions

func display_sprite(en):
	sprite_node.visible = en

func clear_annotations():
	for k in _annotation:
		_annotation[k].hide()
	sprite_node.visible = true

func set_annotation(ref):
	if ref.empty():
		return
	if not _annotation.has(ref):
		print('WARNING: Unknown _annotation ref %s' % [ref])
		return
	_annotation[ref].show()

func clear_line():
	path_node.points = PoolVector2Array([])

# Use global
func set_line(start, end):
	path_node.points = PoolVector2Array([to_local(start), to_local(end)])

# Use global
func set_arc(start, start_dir, end):
	var a = Trig.get_arc(Trig.Ray2D.new(start, start_dir), end)
	if a == null:
		print('Error: invalid arc')
		return
	var pnts = []
	var seg_num = 20
	var seg = a.arc_length/seg_num
	for i in range(seg_num):
		pnts.append(to_local(a.get_point(seg * i)))
	path_node.points = PoolVector2Array(pnts)
