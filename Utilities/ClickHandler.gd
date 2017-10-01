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
	if get_node('Shape').get_shape() is RectangleShape2D:
		get_node('Shape').get_shape().set_extents(tsize/2)
		colshape = Rect2(-tsize.x/2, -tsize.y/2, tsize.x, tsize.y)
	else:
		print('Error: Unknown shape')

	# Enable Engine calbacks
	set_process_input(true)

# Check if mouse within shape
func _input(ev):
	# Skip events outside bounds
	if colshape == null or not colshape.has_point(to_local(ev.position)):
		return

	# Left Click
	if ev is InputEventMouseButton and ev.button_index == BUTTON_LEFT and ev.is_pressed():
		emit_signal("single_click", BUTTON_LEFT)
		get_tree().set_input_as_handled()
	# Right Click
	elif ev is InputEventMouseButton and ev.button_index == BUTTON_RIGHT and ev.is_pressed():
		emit_signal("single_click", BUTTON_RIGHT)
		get_tree().set_input_as_handled()
	else:
		pass

