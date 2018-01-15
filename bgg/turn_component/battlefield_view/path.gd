extends Line2D

const Trig = preload('res://utilities/trig.gd')

func _ready():
	pass

func set_line(start, end):
	points = PoolVector2Array([start, end])

func set_arc(start, end, start_gdir):
	Trig.get_arc()
