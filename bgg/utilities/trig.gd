# This class provides static functions to help with trigonemtry, lines, arcs and directions.

# Types
class Ray2D:
	var offset = Vector2(0,0) setget ,_offset_get
	func _offset_get():
		return offset
	var direction = Vector2(1,0) setget ,_direction_get
	func _direction_get():
		return direction

	func _init(o = Vector2(0,0), d = Vector2(1,0)):
		if typeof(o) != TYPE_VECTOR2 or typeof(d) != TYPE_VECTOR2:
			print('get_dist_to_line: invalid parameters')
		else:
			offset = o
			direction = d.normalized()

	func is_valid():
		if direction.is_normalized():
			return true
		else:
			false

class Arc2D:
	var start_ray = Ray2D.new() setget ,_start_ray_get
	func _start_ray_get():
		return start_ray
	var end_ray = Ray2D.new() setget ,_end_ray_get
	func _end_ray_get():
		return end_ray
	var center = Vector2(0,0) setget ,_center_get
	func _center_get():
		return center
	var radius = 0 setget ,_radius_get
	func _radius_get():
		return radius
	var arc_angle = 0 setget ,_arc_angle_get
	func _arc_angle_get():
		return arc_angle
	var arc_length = 0 setget ,_arc_length_get
	func _arc_length_get():
		return arc_length

	func _init(s, e, c):
		if not s.is_valid() or not e.is_valid() or typeof(c) != TYPE_VECTOR2:
			print('get_dist_to_line: invalid parameters')
		else:
			start_ray = s
			end_ray = e
			center = c
			radius = start_ray.offset.distance_to(center)
			arc_angle = (start_ray.offset - center).angle_to(end_ray.offset - center)
			arc_length = 2 * PI * radius * abs(arc_angle/(2 * PI))

	func is_valid():
		if start_ray.is_valid() and end_ray.is_valid():
			return true
		else:
			false

	func get_point(dist):
		if dist >= arc_length:
			return end_ray.offset

		var angle = (dist / arc_length) * arc_angle;
		var x = cos(angle) * radius
		var y = sin(angle) * radius
		var pnt = Vector2(x, y)

		# Rotate appropriately
		var cord = end_ray.offset - start_ray.offset
		var body_ang = Vector2(0,1).angle_to(start_ray.direction)
		var pnt_ang = start_ray.direction.angle_to(cord)
		if pnt_ang > 0:
			pnt = pnt.rotated(body_ang)
		else:
			pnt = pnt.rotated(body_ang - PI)

		# Add center offset
		pnt.x += center.x
		pnt.y += center.y
		return pnt

enum Orientation { FRONT, BACK, LEFT, RIGHT}

static func get_arc(start_ray, pnt):
	# Invalid of not in front half of start_ray
	if get_dir_relative_to_orientation(start_ray, pnt) != Orientation.FRONT:
		return null

	# LegA of isosceles triangle is perp to dir
	var tri_base_out = (pnt - start_ray.offset).normalized() # base dir
	var tri_base_in = -tri_base_out

	var leg_a
	var clockwise = start_ray.direction.angle_to(tri_base_out) > 0
	if clockwise:
		leg_a = Ray2D.new(start_ray.offset, Vector2(-start_ray.direction.y, start_ray.direction.x))
	else:
		leg_a = Ray2D.new(start_ray.offset, Vector2(start_ray.direction.y, -start_ray.direction.x))
	var leg_ang = leg_a.direction.angle_to(tri_base_out)

	var new_dir
	var final_dir # Perp to leg_b is final facing direction
	if clockwise:
		new_dir = tri_base_in.rotated(-leg_ang)
		final_dir = new_dir.rotated(-PI/2)
	else:
		new_dir = tri_base_in.rotated(leg_ang)
		final_dir = new_dir.rotated(PI/2)
	var leg_b = Ray2D.new(pnt, new_dir)
	var center = get_intersection_of_two_lines(leg_a, leg_b)
	return Arc2D.new(start_ray, Ray2D.new(pnt, final_dir), center)

# Return an array of points that creates a polygon around the given array of pnts with a given width
#
# * pnts - (Array of Vector2) An array of points that make up the line to surround
# * width - (Float) The final width of the polygon surrounding the line
static func get_line_as_polygon(pnts, width):
	var r_area_pnts = []
	var l_area_pnts = []
	var offset_v = null
	for i in range(pnts.size()):
		# Use prev offset for final index
		if i < pnts.size() - 1:
			offset_v = width * (pnts[i + 1] - pnts[i]).normalized().rotated(PI/2)
		r_area_pnts.append(pnts[i] + offset_v)
		l_area_pnts.push_front(pnts[i] - offset_v)
	return r_area_pnts + l_area_pnts

static func get_nearest_pnt_on_line(line, pnt):
	if not line.is_valid() or typeof(pnt) != TYPE_VECTOR2:
		print('get_nearest_pnt_on_line: invalid parameters: %s %s' % [line, pnt])
		return null
	var vect = pnt - line.offset
	var dist = vect.dot(line.direction)
	return line.offset + line.direction * dist

static func get_dist_to_line(line, pnt):
	if not line.is_valid() or typeof(pnt) != TYPE_VECTOR2:
		print('get_dist_to_line: invalid parameters: %s %s' % [line, pnt])
		return null
	var point_on_line = get_nearest_pnt_on_line(line, pnt)
	return pnt.distance_to(point_on_line)

static func get_front_back_dir_relative_to_orientation(orientation_ray, pnt):
	var vec = pnt - orientation_ray.offset
	var ang = orientation_ray.direction.angle_to(vec)
	var abs_ang = abs(ang)
	if abs_ang <= PI/2:
		return Orientation.FRONT
	else:
		return Orientation.BACK

static func get_left_right_dir_relative_to_orientation(orientation_ray, pnt):
	var vec = pnt - orientation_ray.offset
	var ang = orientation_ray.direction.angle_to(vec)
	if ang <= 0:
		return Orientation.RIGHT
	else:
		return Orientation.LEFT

static func get_dir_relative_to_orientation(orientation_ray, pnt):
	var vec = pnt - orientation_ray.offset
	var ang = orientation_ray.direction.angle_to(vec)
	var abs_ang = abs(ang)
	if abs_ang <= PI/4:
		return Orientation.FRONT
	elif abs_ang >= 3*PI/4:
		return Orientation.BACK
	elif ang <= 0:
		return Orientation.RIGHT
	else:
		return Orientation.LEFT

static func get_intersection_of_two_lines(line_1, line_2):

	# Get A,B,C of first line - points : ps1 to pe1
	var ps1 = line_1.offset
	var pe1 = line_1.offset + line_1.direction
	var a1 = pe1.y - ps1.y
	var b1 = ps1.x - pe1.x
	var c1 = a1 * ps1.x + b1 * ps1.y

	# Get A,B,C of second line - points : ps2 to pe2
	var ps2 = line_2.offset
	var pe2 = line_2.offset + line_2.direction
	var a2 = pe2.y - ps2.y
	var b2 = ps2.x - pe2.x
	var c2 = a2 * ps2.x + b2 * ps2.y

	# Get delta and check if the lines are parallel
	var delta = a1 * b2 - a2 * b1
	if delta == 0:
		return null

	return Vector2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta)

static func unit_test():
	print('Running Trig unit-tests')
	var input = []
	var exp_output = []
	var FLOAT_EPSILON = 0.1

	# get_arc(start_ray, pnt)
	input = [
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-.2,-1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-1,.2)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(1,-.2)],
	]
	exp_output = [
		null,
		null,
		null,
	]
	for i in range(input.size()):
		var ret = get_arc(input[i][0], input[i][1])
		if ret != exp_output[i]:
			print('Test get_arc() %d: failed: %s instead of %s' % [i, ret, exp_output[i]])

	# get_nearest_pnt_on_line(line, pnt)
	input = [
		[Ray2D.new(Vector2(0,0), Vector2(1,0)), Vector2(1,1)],
		[Ray2D.new(Vector2(0,0), Vector2(1,0)), Vector2(1,-1)],
	]
	exp_output = [
		Vector2(1,0),
		Vector2(1,0),
	]
	for i in range(input.size()):
		var ret = get_nearest_pnt_on_line(input[i][0], input[i][1])
		if ret != exp_output[i]:
			print('Test get_nearest_pnt_on_line() %d: failed: %s instead of %s' % [i, ret, exp_output[i]])

	# get_dist_to_line(line, pnt)
	input = [
		[Ray2D.new(Vector2(0,0), Vector2(1,0)), Vector2(1,1)],
		[Ray2D.new(Vector2(0,0), Vector2(1,0)), Vector2(1,-1)],
	]
	exp_output = [
		1,
		1,
	]
	for i in range(input.size()):
		var ret = get_dist_to_line(input[i][0], input[i][1])
		if ret != exp_output[i]:
			print('Test get_dist_to_line() %d: failed: %s instead of %s' % [i, ret, exp_output[i]])

	# get_front_back_dir_relative_to_orientation(origin_ray, pnt)
	input = [
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(1,1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-1,1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-1,-1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(1,-.1)],
	]
	exp_output = [
		Orientation.FRONT,
		Orientation.FRONT,
		Orientation.BACK,
		Orientation.BACK,
	]
	for i in range(input.size()):
		var ret = get_front_back_dir_relative_to_orientation(input[i][0], input[i][1])
		if ret != exp_output[i]:
			print('Test get_front_back_dir_relative_to_orientation() %d: failed: %s instead of %s' % [i, ret, exp_output[i]])

	# get_left_right_dir_relative_to_orientation(orientation_ray, pnt)
	input = [
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(1,1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-1,1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-1,-1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(1,-.1)],
	]
	exp_output = [
		Orientation.RIGHT,
		Orientation.LEFT,
		Orientation.LEFT,
		Orientation.RIGHT,
	]
	for i in range(input.size()):
		var ret = get_left_right_dir_relative_to_orientation(input[i][0], input[i][1])
		if ret != exp_output[i]:
			print('Test get_left_right_dir_relative_to_orientation() %d: failed: %s instead of %s' % [i, ret, exp_output[i]])

	# get_dir_relative_to_orientation(origin_ray, pnt)
	input = [
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(.2,1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-.2,-1)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(-1,.2)],
		[Ray2D.new(Vector2(0,0), Vector2(0,1)), Vector2(1,-.2)],
	]
	exp_output = [
		Orientation.FRONT,
		Orientation.BACK,
		Orientation.LEFT,
		Orientation.RIGHT,
	]
	for i in range(input.size()):
		var ret = get_dir_relative_to_orientation(input[i][0], input[i][1])
		if ret != exp_output[i]:
			print('Test get_dir_relative_to_orientation() %d: failed: %s instead of %s' % [i, ret, exp_output[i]])

	# get_intersection_of_two_lines(line_1, line_2)
	input = [
		[Ray2D.new(Vector2(0,0), Vector2(1,0)), Ray2D.new(Vector2(1,1), Vector2(0,-1))],
		[Ray2D.new(Vector2(0,0), Vector2(1,0)), Ray2D.new(Vector2(0,1), Vector2(1,0))],
	]
	exp_output = [
		Vector2(1,0),
		null
	]
	for i in range(input.size()):
		var ret = get_intersection_of_two_lines(input[i][0], input[i][1])
		if ret != exp_output[i]:
			print('Test get_intersection_of_two_lines() %d: failed: %s instead of %s' % [i, ret, exp_output[i]])
