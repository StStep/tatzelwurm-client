extends Node
# class_name WATTest

class State:
	const START: String = "start"
	const PRE: String = "pre"
	const EXECUTE: String = "execute"
	const POST: String = "post"
	const END: String = "end"
	
var _state: String
var _methods: Array = []
var _method: Dictionary = {}
var time: float = 0.0
var _test: Node
signal completed

func _init(test) -> void:
	_test = test

func _ready() -> void:
	_test.Yielder.connect("finished", self, "_next")
	add_child(_test)
	add_child(_test.Yielder)

func _next(vargs = null):
	# When yielding until signals or timeouts, this gets called on resume
	# We call defer here to give the __testcase method time to reach either the end
	# or an extra yield at which point we're able to check the _state of the yield and
	# see if we stay paused or can continue
	call_deferred("_change_state")
	
func _change_state() -> void:
	if _test.Yielder.is_active():
		return
	match _state:
		State.START:
			_pre()
		State.PRE:
			_execute()
		State.EXECUTE:
			_post()
		State.POST:
			_pre()
		State.END:
			_end()
	
func _start():
	_state = State.START
	_test.Start() if _test.get_script() is CSharpScript else _test.start()
	_next()
	
func _pre():
	time = OS.get_ticks_msec()
	if _methods.empty():
		_state = State.END
		_next()
		return
	_state = State.PRE
	_test.Pre() if _test.get_script() is CSharpScript else _test.pre()
	_next()
	
func _execute():
	_state = State.EXECUTE
	if _test.get_script() is GDScript and _test.rerun_method:
		_method = _method
	else:
		_method = _methods.pop_back()
	_test.Testcase.add_method(_method.title)
	_test.callv(_method.title, _method.args) 
	_next()
	
func _post():
	_test.Testcase.methods.back().time = (OS.get_ticks_msec() - time) / 1000.0
	_state = State.POST
	_test.Post() if _test.get_script() is CSharpScript else _test.pre()
	_next()
	
func _end():
	_state = State.END
	_test.End() if _test.get_script() is CSharpScript else _test.end()
	emit_signal("completed")
	
func _exit_tree() -> void:
	if _test.get_script() is GDScript:
		_test.free()
	queue_free()
	
func start():
	pass
	
func pre():
	pass
	
func post():
	pass
	
func end():
	pass
