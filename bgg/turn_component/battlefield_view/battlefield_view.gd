extends Node

export(String, FILE, '*.tscn') var block_scene
export(String, FILE, '*.tscn') var cmd_scene
export(String, FILE, '*.tscn') var path_scene

onready var block_node = load(block_scene)
onready var cmd_node = load(cmd_scene)
onready var path_node = load(path_scene)

const DEFAULT_EOT_MV_EN = false

# A dictionary of lists, representing a queue for unit ref
var _unit_queues = {}

func _ready():
	pass

func clear():
	for k in _unit_queues:
		for q in _unit_queues[k]:
			q.queue_free()
		_unit_queues[k].clear()
	_unit_queues.clear()

func new_unit(ref, gpos, gdir):
	if _unit_queues.has(ref):
		print('WARNING: _unit_queues of ref %s already exists' % [ref])
		return
	# Unit Block
	var inst = block_node.instance()
	add_child(inst)
	inst.global_position = gpos
	inst.global_rotation = gdir.angle() + PI/2
	_unit_queues[ref] = [inst]
	inst.move_ind.visible = DEFAULT_EOT_MV_EN
	_update_eot_move(ref)

# Params = annotation, gdir, visible
func add_cmd(ref, lpos, params = {}):
	if not _unit_queues.has(ref):
		print('WARNING: Unknown _unit_queues ref %s' % [ref])
		return
	# Node
	var par = _unit_queues[ref].back()
	var inst_n = cmd_node.instance()
	par.add_child(inst_n)
	_unit_queues[ref].append(inst_n)
	inst_n.global_position = par.global_position + lpos
	var gdir = params['gdir'] if 'gdir' in params else lpos
	inst_n.global_rotation = gdir.angle() + PI/2
	if 'annotation' in params:
		var a = params['annotation']
		if typeof(a) == TYPE_ARRAY:
			for i in a:
				inst_n.set_annotation(i)
		else:
			inst_n.set_annotation(a)
	if 'visible' in params:
		inst_n.display_sprite(params['visible'])
	# Path
	var inst_p = path_node.instance()
	inst_n.add_child(inst_p)
	inst_p.points = PoolVector2Array([inst_p.to_local(par.global_position), Vector2(0, 0)])
	_update_eot_move(ref)

func display_eot_move(ref, en):
	if not _unit_queues.has(ref):
		print('WARNING: Unknown _unit_queues ref %s' % [ref])
		return
	_unit_queues[ref].front().move_ind.visible = en
	_update_eot_move(ref)

func _update_eot_move(ref):
	var last = _unit_queues[ref].back()
	_unit_queues[ref].front().set_move_ind(last.global_position, last.global_rotation)
