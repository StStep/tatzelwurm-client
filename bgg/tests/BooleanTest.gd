extends WAT.Test

func title() -> String:
	return "Given A Boolean Assertion (GDScript WATTest)"

func testTrue_is_true() -> void:

	asserts.is_true(true, "True is True")

func test_False_is_false() -> void:

	asserts.is_false(false, "False is False")

func test_Value_is_true() -> void:
	parameters([["value"], [true], [1]])

	asserts.is_true(p.value, "%s is true" % p.value)
