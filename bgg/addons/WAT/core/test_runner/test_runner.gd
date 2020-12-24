extends Node

const COMPLETED: String = "completed"
var JunitXML = preload("res://addons/WAT/resources/JUnitXML.gd").new()
var test_loader: Reference = preload("test_loader.gd").new()
var test_results: Resource = preload("res://addons/WAT/resources/results.tres")
var test_double_registry = preload("res://addons/WAT/core/double/registry.gd").new()
var _tests: Array = []
var _cases: Array = []
var _strategy: Dictionary = {}
signal ended

func strategy() -> Dictionary:
	# We consume this so we can avoid worrying about mutatiosn
	var m_strategy = {}
	var strat = ProjectSettings.get_setting("WAT/TestStrategy")
	for key in strat:
		m_strategy[key] = strat[key]
	ProjectSettings.set_setting("WAT/TestStrategy", {})
	ProjectSettings.save()
	m_strategy["repeat"] = m_strategy["repeat"] as int
	return m_strategy

var _time: float
func _ready() -> void:
	_time = OS.get_ticks_msec()
	_strategy = strategy()
	if get_tree().root.get_child(0) == self:
		print("Starting WAT Test Runner")
	OS.window_minimized = ProjectSettings.get_setting(
			"WAT/Minimize_Window_When_Running_Tests")
	_begin()

func _begin():
	_tests = get_tests()

	if _tests.empty():
		push_warning("No Scripts To Test")
	_run_tests()

func get_tests() -> Array:
	match _strategy["strategy"]:
		"RunAll":
			return test_loader.all()
		"RunDirectory":
			return test_loader.directory(_strategy["directory"])
		"RunScript":
			return test_loader.script(_strategy["script"])
		"RunTag":
			return test_loader.tag(_strategy["tag"])
		"RunMethod":
			return test_loader.script(_strategy["script"])
		"RerunFailures":
			return test_loader.last_failed()
		_:
			return _tests


var time_taken: float
func _run_tests() -> void:
	while not _tests.empty():
		yield(run(), COMPLETED)
	_strategy["repeat"] -= 1
	if _strategy["repeat"] > 0:
		call_deferred("_begin")
	else:
		time_taken = _time / 1000.0
		end()

const Executor = preload("res://addons/WAT/core/test/executable.gd")
const TestCase = preload("res://addons/WAT/core/test/case.gd")

func run(test = _tests.pop_front().new()) -> void:
	var testcase: TestCase
	if test.get_script() is CSharpScript:
		testcase = TestCase.new(test.Title(), test.get_script().resource_path as String)
		test.SetUp(testcase)
	elif test.get_script() is GDScript:
		testcase = TestCase.new(test.title(), test.path())
		test.setup(testcase, test_double_registry)
	var executable = Executor.new(test)
	var start_time = OS.get_ticks_msec()
	add_child(executable)
	# Add Strategy Here?
	if _strategy.has("method") and test.get_script() is CSharpScript:
		executable._methods = test.GetTestMethod(_strategy.method)
	elif _strategy.has("method") and test.get_script() is GDScript:
		executable._methods = [{"title": _strategy.method, "args": []}]
	elif test.get_script() is CSharpScript:
		executable._methods = test.GetMethods()
	elif test.get_script() is GDScript:
		executable._methods = test.methods()
	executable._start()
	var time = OS.get_ticks_msec() - start_time
	testcase.time_taken = time / 1000.0
	yield(executable, COMPLETED)
	testcase.calculate()
	_cases.append(testcase.to_dictionary())
	remove_child(executable)


func end() -> void:
	print("Ending WAT Test Runner")
	OS.window_minimized = false
	JunitXML.save(_cases, time_taken)
	test_results.deposit(_cases)
	emit_signal("ended")
	clear()
	if ProjectSettings.get("RunInEngine"):
		get_tree().quit()
	else:
		queue_free()

func clear() -> void:
	pass
#	if ProjectSettings.has_setting("WAT/TestDouble"):
#		ProjectSettings.get_setting("WAT/TestDouble").clear()
#		if ProjectSettings.get("RunInEngine"):
#			var testDouble = ProjectSettings.get_setting("WAT/TestDouble")
			#ProjectSettings.set_setting("WAT/TestDouble", "Disabled")
			#ProjectSettings.get_setting("WAT/TestDouble").free()

