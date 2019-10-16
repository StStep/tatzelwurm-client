# # test_harness.gd
#
# This Scene acts a test harness for BattlefieldView

extends Node

#### Constants

const Trig = preload('res://utilities/trig.gd')

#### Variables

# Item under test
onready var _battlefield_view_node = get_node('BattlefieldView')

# Used to track time between test step executions
var _t_wait = 0
# Used to track function executing as test
var _cur_func = null

#### Private Functions

# Node function, called once all children are ready
func _ready():
	if not _battlefield_view_node:
		print('Error, no BattlefieldView found')
		return
	_battlefield_view_node.connect('mouse_entered_item', self, '_view_mouse_enter')
	_battlefield_view_node.connect('mouse_exited_item', self, '_view_mouse_exit')
	set_process(true)
	Trig.unit_test()

# Node function, called on idle, run coroutine _cur_func
func _process(delta):
	if _cur_func:
		_t_wait += delta
		if _t_wait > .5 and _cur_func.is_valid():
			_t_wait = 0
			_cur_func = _cur_func.resume()

# This is a signal handling function for battlefield view mouse enter action
#
# * ref - (String) Set on connection, the string for the unit_ref assocaited with the rep
# * ind - (Int) The index into the unit_queue for the rep
# * is_path - (Bool) Set by the signal, true if the rep path is the source
func _view_mouse_enter(ref, ind, is_path):
	print('Mouse entered %s of index %d and is_path == %s' % [ref, ind, is_path])

# This is a signal handling function for battlefield view mouse exit action
#
# * ref - (String) Set on connection, the string for the unit_ref assocaited with the rep
# * ind - (Int) The index into the unit_queue for the rep
# * is_path - (Bool) Set by the signal, true if the rep path is the source
func _view_mouse_exit(ref, ind, is_path):
	print('Mouse exited %s of index %d and is_path == %s' % [ref, ind, is_path])

# Testing mutliple annoations at once
func _test_1():
	print('Running Test One...')
	var unit_ref = 'unit_1'

	# Clear
	_battlefield_view_node.clear()
	yield()

	# Add Unit
	_battlefield_view_node.new_unit(unit_ref, Vector2(200,200), Vector2(1,1))
	yield()

	# Add Generic Command 1
	_battlefield_view_node.add_cmd(unit_ref, Vector2(330,330))
	yield()

	# Add Generic Command 2
	_battlefield_view_node.add_cmd(unit_ref, Vector2(270,390),
			{'annotation' :  ['reposition', 'rotation'], 'end_gdir' :  Vector2(.5,1)})
	yield()

	print('Completed Test One')
	return null

# Test rotation command annotation and invalid highlighting
func _test_2():
	print('Running Test Two...')
	var unit_ref = 'unit_2'

	# Clear
	_battlefield_view_node.clear()
	yield()

	# Add Unit
	_battlefield_view_node.new_unit(unit_ref, Vector2(815,541), Vector2(1,-1))
	_battlefield_view_node.display_eot_move(unit_ref, true)
	yield()

	# Add Generic Command 1
	_battlefield_view_node.add_cmd(unit_ref, Vector2(945,410),
			{'annotation' : 'rotation', 'end_gdir' : Vector2(-130,-130)})
	yield()

	# Color Command 1and last half of path as Invalid
	_battlefield_view_node.highlight_cmd_path(unit_ref, 1, 'Invalid', .5)
	_battlefield_view_node.highlight_cmd_body(unit_ref, 1, 'Invalid')

	# Add Generic Command 2
	_battlefield_view_node.add_cmd(unit_ref, Vector2(815,280))
	yield()

	print('Completed Test Two')
	return null

# Test Wheel and Reposition commands
func _test_3():
	print('Running Test Three...')
	var unit_ref = 'unit_3'

	# Clear
	_battlefield_view_node.clear()
	yield()

	# Add Unit
	_battlefield_view_node.new_unit(unit_ref, Vector2(200,200), Vector2(1,1))
	_battlefield_view_node.display_eot_move(unit_ref, true)
	yield()

	# Color Unit as focused
	_battlefield_view_node.highlight_cmd_path(unit_ref, 1, 'Focus', Vector2(.5, 1))
	_battlefield_view_node.highlight_cmd_body(unit_ref, 0, 'Focus')

	# Add Generic Command 1
	_battlefield_view_node.add_cmd(unit_ref, Vector2(330,330))
	yield()

	# Add Generic Command 2
	_battlefield_view_node.add_cmd(unit_ref, Vector2(335,340), {'annotation' : 'wheel'})
	yield()

	# Add Generic Command 3
	_battlefield_view_node.add_cmd(unit_ref, Vector2(385,240),
			{'annotation' : 'reposition', 'end_gdir' : Vector2(5,10)})
	yield()

	# Add Generic Command 4
	_battlefield_view_node.add_cmd(unit_ref, Vector2(395,245), {'annotation' : 'wheel'})
	yield()

	print('Completed Test Three')
	return null

# Test drawing an arc
func _test_4():
	print('Running Test Four...')
	var unit_ref = 'unit_4'

	# Clear
	_battlefield_view_node.clear()
	yield()

	# Add Unit
	_battlefield_view_node.new_unit(unit_ref, Vector2(200,200), Vector2(1,1))
	_battlefield_view_node.display_eot_move(unit_ref, true)
	yield()

	# Add Generic Command 1
	_battlefield_view_node.add_cmd(unit_ref, Vector2(330,330))
	yield()

	# Add Generic Command 2
	_battlefield_view_node.add_cmd(unit_ref, Vector2(460, 370), {'arc_gdir' : Vector2(1,1), 'end_gdir' : Vector2(0.982305, -0.187288)})
	yield()

	print('Completed Test Four')
	return null

#### Public Functionss

# Set _cur_func to _test_1, if none is set
func test_1():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_1()

# Set _cur_func to _test_2, if none is set
func test_2():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_2()

# Set _cur_func to _test_3, if none is set
func test_3():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_3()

# Set _cur_func to _test_4, if none is set
func test_4():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_4()
