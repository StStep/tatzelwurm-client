# # battlefield_veiw.gd
#
# This node acts as the renderer for the units on the battlefied and the
# commands they have been given.
#
# ## Primary Functions
#
# * clear() - Remove all previously added units and commands
# * new_unit() - Add a unit to be rendered
# * add_cmd() - Add a command to a unit to be rendered
# * display_eot_move() - Enable/Disable the end-of-turn movement indicator for a unit
#
# ## Instanced Scenes:
#
# * unit_rep.tscn - Represents a unit on the field, includes the move indictor
# * cmd_rep.tscn - Represents a command given to unit, includes the preceding path
#
# ## Signals
#
# * mouse_entered_item - Given when a mouse enters the collision area of a represented item
# * mouse_exited_item - Given when a mouse exits the collision area of a represented item

extends Node

#### Constants

const DEFAULT_EOT_MV_EN = false

#### Signals

# These signals are given if a mouse enters or exits a unit or command representative item
#
# * ref - (String) Unit reference of item
# * ind - Index into queue of item, index 0 is unit_rep, otherwise its a cmd_rep
# * is_path - True if the proceding path is of interest, instead of the item body.
#       For the unit_rep, this is instead the EOT movement indicator.
# <<<<
signal mouse_entered_item(unit_ref, ind, is_path)
signal mouse_exited_item(unit_ref, ind, is_path)
# >>>>

#### Variables

## Instance Scene file-paths and nodes
# <<<<
export(String, FILE, '*.tscn') var unit_rep_scene
export(String, FILE, '*.tscn') var cmd_rep_scene

onready var _unit_rep_node = load(unit_rep_scene)
onready var _cmd_rep_node = load(cmd_rep_scene)
# >>>>

# A dictionary of lists, representing a queue for unit references
# Queue index 0 = unit rep, with following being cmd reps
var _unit_queues = {}

#### Private Functions

# Node function, called once all children are ready
func _ready():
	pass

# Move the EOT move indicator to the position/rotation of last command in queue
func _update_eot_move(ref):
	var last = _unit_queues[ref].back()
	_unit_queues[ref].front().SetMoveIndicator(last.global_position, last.global_rotation)

# This is a signal handling function for unit and move representative mouse actions
#
# * is_path - (Bool) Set by the signal, true if the rep path is the source
# * is_enter - (Bool) Set on connection, true if the action is a mouse entering, otherwise false
# * ref - (String) Set on connection, the string for the unit_ref assocaited with the rep
# * ind - (Int) The index into the unit_queue for the rep
func _rep_mouse_action(is_path, is_enter, ref, ind):
	if is_enter:
		emit_signal('mouse_entered_item', ref, ind, is_path)
	else:
		emit_signal('mouse_exited_item', ref, ind, is_path)

#### Public Functions

# Remove all previously added units and commands
func clear():
	for k in _unit_queues:
		for q in _unit_queues[k]:
			q.queue_free()
		_unit_queues[k].clear()
	_unit_queues.clear()

# Add a unit to be rendered
#
# * ref - (String) Unit reference to place the new unit under
# * gpos - (Vector2) Global position for unit on battlefield
# * gdir - (Vector2) A vector pointing in a global direction to face the representation
func new_unit(ref, gpos, gdir):
	if _unit_queues.has(ref):
		print('WARNING: _unit_queues of ref %s already exists' % [ref])
		return
	# Unit Block
	var inst = _unit_rep_node.instance()
	inst.SelectManager = $SelectManager
	add_child(inst)
	inst.global_position = gpos
	inst.global_rotation = gdir.angle() + PI/2
	_unit_queues[ref] = [inst]
	inst.SetMoveIndicatorVisibility(DEFAULT_EOT_MV_EN)
	_update_eot_move(ref)

# Add a command to a unit to be rendered
#
# * ref - (String) Unit reference to place the command representation under
# * gpos - (Vector2) Global position for the command on battlefield
# * params - (dict) A dictionary that optionally has the following entries
#
#     * annotation - (String or [Strings]) Enable displayed annoations, see cmd_rep.gd
#     * end_gdir - (Vector2) Vector pointing the global direction for command
#           facing, defualts to directly away from previous unit queue entry
#     * visible - (Bool) If false show only the cmd rep path, else show everything, defualts to true
#     * arc_gdir - (Vector2) If given, arc the path for the command with the given
#           initial global direction vector
func add_cmd(ref, gpos, params = {}):
	if not _unit_queues.has(ref):
		print('WARNING: Unknown _unit_queues ref %s' % [ref])
		return
	# Command
	var par = _unit_queues[ref].back()
	var inst = _cmd_rep_node.instance()
	par.add_child(inst)
	_unit_queues[ref].append(inst)
	inst.global_position = gpos
	var gdir = params['end_gdir'] if 'end_gdir' in params else gpos - par.global_position
	inst.global_rotation = gdir.angle() + PI/2
	if 'annotation' in params:
		var a = params['annotation']
		if typeof(a) == TYPE_ARRAY:
			for i in a:
				inst.add_annotation(i)
		else:
			inst.add_annotation(a)
	if 'visible' in params:
		inst.display_cmd(params['visible'])
	# Path
	if 'arc_gdir' in params:
		inst.set_path_as_arc(par.global_position, params['arc_gdir'])
	else:
		inst.set_path_as_line(par.global_position)
	# Connections
	inst.connect('mouse_entered_item', self, '_rep_mouse_action', [true, ref, _unit_queues[ref].size() - 1])
	inst.connect('mouse_exited_item', self, '_rep_mouse_action', [false, ref, _unit_queues[ref].size() - 1])
	_update_eot_move(ref)

# Enable/Disable the end-of-turn movement indicator for a unit
#
# * ref - (String) Unit reference to modify
# * en - (Vector2) True if the indicator should be displayed, otherwise false
func display_eot_move(ref, en):
	if not _unit_queues.has(ref):
		print('WARNING: Unknown _unit_queues ref %s' % [ref])
		return
	_unit_queues[ref].front().SetMoveIndicatorVisibility(en)
	_update_eot_move(ref)

# Highlight the body of a command, for a given type
#
# * ref - (String) Unit reference to modify
# * ind - (Int) The index into the unit_queue for the rep
# * type - (String) The type of highlight being requested [None, Focus, Invalid, Inactive]
func highlight_cmd_body(ref, ind, type):
	if not _unit_queues.has(ref) or _unit_queues[ref].size() <= ind:
		print('WARNING: Unknown _unit_queues ref %s with index %d' % [ref, ind])
		return
	_unit_queues[ref][ind].highlight_body(type)

# Highlight the path of a cmd rep, for a given type and coverage
#
# * ref - (String) Unit reference to modify
# * ind - (Int) The index into the unit_queue for the rep
# * type - (String or String List) The type of highlight being requested for coverage n [None, Focus, Invalid, Inactive]
# * coverage - (float or float list) The portion of the path to cover for type n, values should be between 0 and 1
func highlight_cmd_path(ref, ind, type, coverage = 1):
	if not _unit_queues.has(ref) or _unit_queues[ref].size() <= ind:
		print('WARNING: Unknown _unit_queues ref %s with index %d' % [ref, ind])
		return
	var t = type if typeof(type) == TYPE_ARRAY else [type]
	var c = coverage if typeof(coverage) == TYPE_ARRAY else [coverage]
	_unit_queues[ref][ind].highlight_path(t, c)
