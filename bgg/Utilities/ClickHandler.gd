# Allows for Area2D to handle clicks, and send signals
#
# Requires child Shape (Collision2D) and Sprite (Sprite2D)
# Auto-detects Shape2D being used with collider for click detection

extends Area2D

signal single_click


var colshape = null # Click area for _input handling


func _ready():
	# Use sprite size for collider
	var tsize = get_node('Sprite').get_texture().get_size()

	# If rect use rect2D
	if get_node('Shape').get_shape().is_type('RectangleShape2D'):
		get_node('Shape').get_shape().set_extents(tsize/2)
		colshape = Rect2(-tsize.x/2, -tsize.y/2, tsize.x, tsize.y)
	else:
		print('Error: Unknown shape')

	# Enable Engine calbacks
	set_process_input(true)

# Check if mouse within shape, rotate mouse instead of shape
func _input(ev):
	# Skip events outside bounds
	var lpos = get_global_pos() - ev.pos
	var lrpos = lpos.rotated(-get_global_rot())
	if colshape == null or not colshape.has_point(lrpos):
		return

	# Left Click
	if ev.type == InputEvent.MOUSE_BUTTON and ev.button_index == BUTTON_LEFT and ev.is_pressed():
		emit_signal("single_click", BUTTON_LEFT)
		get_tree().set_input_as_handled()
	# Right Click
	elif ev.type == InputEvent.MOUSE_BUTTON and ev.button_index == BUTTON_RIGHT and ev.is_pressed():
		emit_signal("single_click", BUTTON_RIGHT)
		get_tree().set_input_as_handled()
	else:
		pass
