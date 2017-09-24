extends Area2D

# Requires child Shape (Collision2D) and Sprite (Sprite2D)
# Auto-detects Shape2D being used with collider for click detection

signal left_click

var colshape = null

func _ready():
	# Set collider to sprite size
	var tsize = get_node('Sprite').get_texture().get_size()
	if get_node('Shape').get_shape().is_type('RectangleShape2D'):
		get_node('Shape').get_shape().set_extents(tsize/2)
		colshape = Rect2(-tsize.x/2, -tsize.y/2, tsize.x, tsize.y)
	else:
		print('Error: Unknown shape')

	# Enable Engine calbacks
	set_process_input(true)

func _input(ev):
	# Check if mouse within shape, rotate mouse instead of shape
	if ev.type == InputEvent.MOUSE_BUTTON and ev.button_index == BUTTON_LEFT and ev.is_pressed():
		var lpos = get_global_pos() - ev.pos
		var lrpos = lpos.rotated(-get_global_rot())
		if colshape != null and colshape.has_point(lrpos):
			emit_signal("left_click")
			get_tree().set_input_as_handled()
