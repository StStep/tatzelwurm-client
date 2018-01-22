extends Node

onready var move_ind = get_node('MoveIndicator') setget ,_move_ind_get
func _move_ind_get():
	return move_ind

func _ready():
	pass

func set_move_ind(gpos, grot):
	move_ind.global_position = gpos
	move_ind.global_rotation = grot
