extends Node

onready var battlefield_view = get_node('BattlefieldView')

var _t_wait = 0
var _cur_func = null

func _ready():
	if not battlefield_view:
		print('Error, no BattlefieldView found')
		return
	set_process(true)

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
	battlefield_view.add_cmd(unit_ref, Vector2(130,130))
	yield()

	# Add Generic Command 2
	battlefield_view.add_cmd(unit_ref, Vector2(-130,130))
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
	battlefield_view.add_cmd(unit_ref, Vector2(130,-130))
	yield()

	# Add Generic Command 2
	battlefield_view.add_cmd(unit_ref, Vector2(-130,-130))
	yield()

	print('Completed Test Two')
	return null
