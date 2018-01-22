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
# * cmd_rep.tscn - Represents a command given to unit
# * path.tscn - Represents the movement of a unit, one precedes each represented command
#
# ## Signals
#
# * mouse_entered_item - Given when a mouse enters the collision
# * mouse_exited_item

extends Node

#### Constants

const DEFAULT_EOT_MV_EN = false

#### Signals

# These signals are given if a mouse enters or exits a unit or command representative: item
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
export(String, FILE, '*.tscn') var path_scene

onready var _unit_rep_node = load(unit_rep_scene)
onready var _cmd_rep_node = load(cmd_rep_scene)
onready var _path_node = load(path_scene)
# >>>>

# A dictionary of lists, representing a queue for unit references
# Queue index 0 = unit rep, with following being cmd reps
var _unit_queues = {}

#### Private Function

# Node function, called once all children are ready
func _ready():
	pass

# Move the EOT move indicator to the position/rotation of last command in queue
func _update_eot_move(ref):
	var last = _unit_queues[ref].back()
	_unit_queues[ref].front().set_move_ind(last.global_position, last.global_rotation)

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
	add_child(inst)
	inst.global_position = gpos
	inst.global_rotation = gdir.angle() + PI/2
	_unit_queues[ref] = [inst]
	inst.move_ind.visible = DEFAULT_EOT_MV_EN
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
#     * visible - (Bool) If true show path, else hide path, defualts to true
#     * arc_gdir - (Vector2) If given, arc the path for the command with the given
#           initial global direction vector
func add_cmd(ref, gpos, params = {}):
	if not _unit_queues.has(ref):
		print('WARNING: Unknown _unit_queues ref %s' % [ref])
		return
	# Command
	var par = _unit_queues[ref].back()
	var inst_c = _cmd_rep_node.instance()
	par.add_child(inst_c)
	_unit_queues[ref].append(inst_c)
	inst_c.global_position = gpos
	var gdir = params['end_gdir'] if 'end_gdir' in params else gpos - par.global_position
	inst_c.global_rotation = gdir.angle() + PI/2
	if 'annotation' in params:
		var a = params['annotation']
		if typeof(a) == TYPE_ARRAY:
			for i in a:
				inst_c.set_annotation(i)
		else:
			inst_c.set_annotation(a)
	if 'visible' in params:
		inst_c.display_sprite(params['visible'])
	# Path
	var inst_p = _path_node.instance()
	inst_c.add_child(inst_p)
	if 'arc_gdir' in params:
		inst_p.set_arc(par.global_position, params['arc_gdir'], inst_c.global_position)
	else:
		inst_p.set_line(par.global_position, inst_c.global_position)
	_update_eot_move(ref)

# Enable/Disable the end-of-turn movement indicator for a unit
#
# * ref - (String) Unit reference to modify
# * en - (Vector2) True if the indicator should be displayed, otherwise false
func display_eot_move(ref, en):
	if not _unit_queues.has(ref):
		print('WARNING: Unknown _unit_queues ref %s' % [ref])
		return
	_unit_queues[ref].front().move_ind.visible = en
	_update_eot_move(ref)
