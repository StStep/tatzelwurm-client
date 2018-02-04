# cmd_rep.gd
#
# This Scene renders a command in the battlefield view
#
# The command representation includes the command and the path.
# The command can have annotations, while the path can be set to bridge two points.

extends Node2D

#### Constants

const Trig = preload('res://utilities/trig.gd')
const PATH_AREA_WIDTH = 10

# Highlight color options
# <<<<
const COLOR_NONE = Color('#ffffff')
const COLOR_FOCUS= Color('#eef442')
const COLOR_INVALID = Color('#e2342b')
const COLOR_INACTIVE = Color('#b2b2b2')
# >>>>

#### Signals

# These signals are given if a mouse enters or exits the command representative
#
# * is_path - True if the the path is the source of the signal, otherwise false
# <<<<
signal mouse_entered_item(is_path)
signal mouse_exited_item(is_path)
# >>>>

#### Variables

# Children Nodes
# <<<<
onready var _sprite_node = get_node('Sprite')
onready var _reposition_node = get_node('Reposition')
onready var _wheel_node = get_node('Wheel')
onready var _rotation_node = get_node('Rotation')
onready var _path_node = get_node('Path')
onready var _path_area_node = get_node('PathArea')
onready var _path_area_poly_node = get_node('PathArea/Polygon')
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
	get_node('BodyArea').connect('mouse_entered', self, '_rep_mouse_action', [true, false])
	get_node('BodyArea').connect('mouse_exited', self, '_rep_mouse_action', [false, false])
	get_node('PathArea').connect('mouse_entered', self, '_rep_mouse_action', [true, true])
	get_node('PathArea').connect('mouse_exited', self, '_rep_mouse_action', [false, true])

# Emit a mouse action depending upon parameters
#
# * is_enter - (Bool) If true emit a mouse_entered_item signal, else emit mouse_exited_item
# * is_path - (Bool) What to give as the is_path signal parameter
func _rep_mouse_action(is_enter, is_path):
	if is_enter:
		emit_signal('mouse_entered_item', is_path)
	else:
		emit_signal('mouse_exited_item', is_path)

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
	_path_area_poly_node.polygon = PoolVector2Array([])

# Set the path as a line between the start and the cmd_rep position
#
# * start - (Vector2) The global position that the path starts at
func set_path_as_line(start):
	var pnts = [to_local(start), Vector2(0,0)]
	_path_node.points = PoolVector2Array(pnts)
	_path_area_poly_node.polygon = PoolVector2Array(Trig.get_line_as_polygon(pnts, PATH_AREA_WIDTH))

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
	_path_area_poly_node.polygon = PoolVector2Array(Trig.get_line_as_polygon(pnts, PATH_AREA_WIDTH))

# Highlight the body for a given type
#
# * type - (String) The type of highlight being requested [None, Focus, Invalid, Inactive]
func highlight_body(type):
	match type:
		'None':
			$Sprite.modulate = COLOR_NONE
		'Focus':
			$Sprite.modulate = COLOR_FOCUS
		'Invalid':
			$Sprite.modulate = COLOR_INVALID
		'Inactive':
			$Sprite.modulate = COLOR_INACTIVE
		_:
			print('WARNING: Unknown highlight type %s' % [type])

# Highlight the path for a given type and coverage
#
# * type - (String) The type of highlight being requested [None, Focus, Invalid, Inactive]
# * coverage - (Vector2) The portion of the path to cover, values should be between 0 and 1 and ascending
func highlight_path(type, coverage):
	print('Debug: highlight_path not implemented')
