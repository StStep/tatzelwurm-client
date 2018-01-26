# unit_rep.gd
#
# This Scene renders a unit in the battlefield view
#
# The End-of-Turn (EOT) Move Indictor indicates that the unit will be
# continuing to move at the end of the turn.

extends Node

#### Signals

# TODO

#### Variables

# Cildren nodes
onready var _move_ind_node = get_node('MoveIndicator')

#### Private Functions

# Node function, called once all children are ready
func _ready():
	pass

#### Public Functions

# Enable/Disable the move indicator being displayed
#
# * en - (Bool) True if the move indicator should be displayed, otherwise false
func display_move_ind(en):
	_move_ind_node.visible = en

# Change the position and rotation of the move indicator to the given global values
#
# * gpos - (Vector2) The global position to place the move indicator at
# * grot - (Vector2) The global rotation to give the move indicator
func place_move_ind(gpos, grot):
	_move_ind_node.global_position = gpos
	_move_ind_node.global_rotation = grot
