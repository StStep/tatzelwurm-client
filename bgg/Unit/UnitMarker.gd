extends Area2D

onready var unit = get_parent()
var tsize = null

func _ready():
	# Set collider to sprite size
	tsize=get_node("Sprite").get_texture().get_size()
	get_node("CollisionShape2D").get_shape().set_extents(tsize/2)

	# Enable Engine calbacks
	set_process_input(true)
	set_process(true)

func _process(delta):
	pass

func _input(ev):
	if ev.type == InputEvent.MOUSE_BUTTON and ev.button_index == BUTTON_LEFT and ev.is_pressed():
		var evpos = ev.global_pos
		var gpos = get_global_pos()
		var spriterect = Rect2(gpos.x-tsize.x/2, gpos.y-tsize.y/2, tsize.x, tsize.y)
		if spriterect.has_point(evpos):
			_clicked()
			get_tree().set_input_as_handled()

func _clicked():
	get_node("/root/game_manager").selUnit = unit