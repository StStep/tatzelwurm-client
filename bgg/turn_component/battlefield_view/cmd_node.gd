extends Node

onready var sprite_node = get_node('Sprite')
onready var reposition_node = get_node('Reposition')
onready var wheel_node = get_node('Wheel')
onready var rotation_node = get_node('Rotation')

onready var _annotation = {
	'reposition' : reposition_node,
	'wheel' : wheel_node,
	'rotation' : rotation_node,
}

func _ready():
	clear_annotations()

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

