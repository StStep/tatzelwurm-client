# Allows for Area2D to handle clicks, and send signals
#
# Requires child Shape (Collision2D) and Sprite (Sprite2D)
# Auto-detects Shape2D being used with collider for click detection

extends Area2D

signal single_click

class Circ2:
	var radius = 0
	var pos = Vector2(0, 0)

	func _init(x, y, r):
		pos = Vector2(x, y)
		radius = r

	func has_point(pnt):
		if abs(pos.distance_to(pnt)) <= radius:
			return true
		else:
			return false


var colshape = null # Click area for _input handling


func _ready():
	# Use sprite size for collider
	var tsize = get_node('Sprite').get_texture().get_size()

	# If rect use rect2D
	if get_node('Shape').get_shape() is RectangleShape2D:
		get_node('Shape').get_shape().set_extents(tsize/2)
		colshape = Rect2(-tsize.x/2, -tsize.y/2, tsize.x, tsize.y)
	# If circle ct use rect2D
	elif get_node('Shape').get_shape() is CircleShape2D:
		get_node('Shape').get_shape().radius = tsize.x/2
		colshape = Circ2.new(0, 0, tsize.x/2)
	else:
		print('Error: Unknown shape')

func Enable():
	# Enable Engine calbacks
	pass

func Disable():
	# Disable Engine calbacks
	pass