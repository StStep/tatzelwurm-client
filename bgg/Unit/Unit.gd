extends Node2D


func _ready():
	# Called every time the node is added to the scene.
	# Initialization here
	pass

func Select():
	print('Selected ' + get_name())

func Deselect():
	print('Deselected ' + get_name())
