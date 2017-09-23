extends Node2D

var IsSelected = false

func _ready():
	# Called every time the node is added to the scene.
	# Initialization here
	pass

func Select():
	IsSelected = true
	get_node('Unit Marker/Sprite').set_modulate(Color('f6ff00')) # Yellow
	print('Selected ' + get_name())

func Deselect():
	IsSelected = false
	get_node('Unit Marker/Sprite').set_modulate('ffffff') # White
	print('Deselected ' + get_name())
