# cmd_rep.gd
#
# This Scene renders a command in the battlefield view
#
# The command representation includes the command and the path.
# The command can have annotations, while the path can be set to bridge two points.

extends Node

#### Constants

const Trig = preload('res://utilities/trig.gd')

#### Signals

# TODO

#### Variables

# Children Nodes
# <<<<
onready var _sprite_node = get_node('Sprite')
onready var _reposition_node = get_node('Reposition')
onready var _wheel_node = get_node('Wheel')
onready var _rotation_node = get_node('Rotation')
onready var _path_node = get_node('Path')
# >>>>

# Possible annotations that can be displayed
onready var _annotation = {
	'reposition' : _reposition_node,
	'wheel' : _wheel_node,
	'rotation' : _rotation_node,
}

#### Private Functions

# Node function, called once all children are ready
func _ready():
	clear_annotations()

#### Public Functions

# Disable/Enable the command, leaving the path untouched
#
# * en - (bool) True to display, otherwise false
func display_cmd(en):
	_sprite_node.visible = en

# Remove all annotations
func clear_annotations():
	for k in _annotation:
		_annotation[k].hide()
	_sprite_node.visible = true

# Add the annotation for the given reference
#
# * ref - (String) One of the possible strings:
#
#     * reposition
#     * wheel
#     * rotation
func add_annotation(ref):
	if ref.empty():
		return
	if not _annotation.has(ref):
		print('WARNING: Unknown _annotation ref %s' % [ref])
		return
	_annotation[ref].show()

# Remove the path, clearing any previous setting
func clear_path():
	_path_node.points = PoolVector2Array([])

# Set the path as a line between the start and the cmd_rep position
#
# * start - (Vector2) The global position that the path starts at
func set_path_as_line(start):
	_path_node.points = PoolVector2Array([to_local(start), Vector2(0,0)])

# Set the path as an arc between the start and the cmd_rep position, with a start direction
#
# * start - (Vector2) The global position that the path starts at
# * start_dir - (Vector2) The global direction that the path starts at
func set_path_as_arc(start, start_dir):
	var a = Trig.get_arc(Trig.Ray2D.new(start, start_dir), to_global(Vector2(0,0)))
	if a == null:
		print('Error: invalid arc')
		return
	var pnts = []
	var seg_num = 20
	var seg = a.arc_length/seg_num
	for i in range(seg_num):
		pnts.append(to_local(a.get_point(seg * i)))
	_path_node.points = PoolVector2Array(pnts)
