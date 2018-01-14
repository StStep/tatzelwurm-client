extends Line2D

func _ready():
	pass

func set_line(start, end):
	points = PoolVector2Array([start, end])

# TODO Determine params and hookinto battlefield_view
func set_arc(start, end, gdir):
	pass
