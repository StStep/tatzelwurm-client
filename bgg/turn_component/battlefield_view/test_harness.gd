extends Node

const Trig = preload('res://utilities/trig.gd')

onready var battlefield_view = get_node('BattlefieldView')

var _t_wait = 0
var _cur_func = null

func _ready():
	if not battlefield_view:
		print('Error, no BattlefieldView found')
		return
	set_process(true)
	Trig.unit_test()

func _process(delta):
	if _cur_func:
		_t_wait += delta
		if _t_wait > .5 and _cur_func.is_valid():
			_t_wait = 0
			_cur_func = _cur_func.resume()

func test_1():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_1()

func _test_1():
	print('Running Test One...')
	var unit_ref = 'unit_1'

	# Clear
	battlefield_view.clear()
	yield()

	# Add Unit
	battlefield_view.new_unit(unit_ref, Vector2(200,200), Vector2(1,1))
	yield()

	# Add Generic Command 1
	battlefield_view.add_cmd(unit_ref, Vector2(330,330))
	yield()

	# Add Generic Command 2
	battlefield_view.add_cmd(unit_ref, Vector2(270,390),
			{'annotation' :  ['reposition', 'rotation'], 'end_gdir' :  Vector2(.5,1)})
	yield()

	# Mark as moving at end-of-turn
	battlefield_view.display_eot_move(unit_ref, true)

	print('Completed Test One')
	return null

func test_2():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_2()

func _test_2():
	print('Running Test Two...')
	var unit_ref = 'unit_2'

	# Clear
	battlefield_view.clear()
	yield()

	# Add Unit
	battlefield_view.new_unit(unit_ref, Vector2(815,541), Vector2(1,-1))
	battlefield_view.display_eot_move(unit_ref, true)
	yield()

	# Add Generic Command 1
	battlefield_view.add_cmd(unit_ref, Vector2(945,410),
			{'annotation' : 'rotation', 'end_gdir' : Vector2(-130,-130)})
	yield()

	# Add Generic Command 2
	battlefield_view.add_cmd(unit_ref, Vector2(815,280))
	yield()

	print('Completed Test Two')
	return null

func test_3():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_3()

func _test_3():
	print('Running Test Three...')
	var unit_ref = 'unit_3'

	# Clear
	battlefield_view.clear()
	yield()

	# Add Unit
	battlefield_view.new_unit(unit_ref, Vector2(200,200), Vector2(1,1))
	battlefield_view.display_eot_move(unit_ref, true)
	yield()

	# Add Generic Command 1
	battlefield_view.add_cmd(unit_ref, Vector2(330,330))
	yield()

	# Add Generic Command 2
	battlefield_view.add_cmd(unit_ref, Vector2(335,340), {'annotation' : 'wheel'})
	yield()

	# Add Generic Command 3
	battlefield_view.add_cmd(unit_ref, Vector2(385,240),
			{'annotation' : 'reposition', 'end_gdir' : Vector2(5,10)})
	yield()

	battlefield_view.add_cmd(unit_ref, Vector2(395,245), {'annotation' : 'wheel'})
	yield()

	print('Completed Test Three')
	return null

func test_4():
	if _cur_func:
		print('Wait for previous test to complete')
		return
	_cur_func = _test_4()

func _test_4():
	print('Running Test Four...')
	var unit_ref = 'unit_4'

	# Clear
	battlefield_view.clear()
	yield()

	# Add Unit
	battlefield_view.new_unit(unit_ref, Vector2(200,200), Vector2(1,1))
	battlefield_view.display_eot_move(unit_ref, true)
	yield()

	# Add Generic Command 1
	battlefield_view.add_cmd(unit_ref, Vector2(330,330))
	yield()

	# Add Arc
	battlefield_view.add_cmd(unit_ref, Vector2(460, 370), {'arc_gdir' : Vector2(1,1), 'end_gdir' : Vector2(0.982305, -0.187288)})
	yield()

	print('Completed Test Four')
	return null
