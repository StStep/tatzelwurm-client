extends Line2D

const Trig = preload('res://utilities/trig.gd')

func _ready():
	pass

# Use global
func set_line(start, end):
	points = PoolVector2Array([to_local(start), to_local(end)])

# Use global
func set_arc(start, start_dir, end):
	var a = Trig.get_arc(Trig.Ray2D.new(start, start_dir), end)
	if a == null:
		print('Error: invalid arc')
		return
	var pnts = []
	var seg_num = 20
	var seg = a.arc_length/seg_num
	for i in range(seg_num):
		pnts.append(to_local(a.get_point(seg * i)))
	points = PoolVector2Array(pnts)