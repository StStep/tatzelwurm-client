# unit_rep.gd
#
# This Scene renders a unit in the battlefield view

extends Node

#### Signals

# TODO

#### Variables

onready var _move_ind_node = get_node('MoveIndicator')

#### Private Function

func _ready():
	pass

#### Public Functions

func display_move_ind(en):
	_move_ind_node.visible = en

func set_move_ind(gpos, grot):
	_move_ind_node.global_position = gpos
	_move_ind_node.global_rotation = grot
