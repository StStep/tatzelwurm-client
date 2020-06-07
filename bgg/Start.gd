extends CanvasLayer

func _on_Start_pressed():
	var _u = get_tree().change_scene("res://PreBattle.tscn")

func _on_Exit_pressed():
	get_tree().quit()
