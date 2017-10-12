extends Node2D

var start = Vector2() setget _setStart, _getStart

func _ready():
	pass

func _setStart(value):
	start = to_local(value)
	get_node("Path").points = PoolVector2Array([start, Vector2(0,0)])

func _getStart():
	return start
