extends Node2D


func _ready():
	# Called every time the node is added to the scene.
	# Initialization here
	pass

func Select():
	get_node('Unit Marker/Sprite').set_modulate(Color('f6ff00')) # Yellow
	print('Selected ' + get_name())

func Deselect():
	get_node('Unit Marker/Sprite').set_modulate('ffffff') # White
	print('Deselected ' + get_name())
