extends Area2D

func _ready():
	
	# Set collider to sprite size
	var tsize=get_node("Sprite").get_texture().get_size()
	get_node("CollisionShape2D").get_shape().set_extents(tsize/2)
	
	# Enable Engine calbacks
	set_process_input(true)
	set_process(true)

func _process(delta):
	pass

func _input_event(viewport, event, shape_idx):
	if event.type == InputEvent.MOUSE_BUTTON \
	and event.button_index == BUTTON_LEFT \
	and event.pressed:
		print("Clicked")
		return(self) # returns a reference to this node
