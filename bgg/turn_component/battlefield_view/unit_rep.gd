# unit_rep.gd
#
# This Scene renders a unit in the battlefield view
#
# The End-of-Turn (EOT) Move Indictor indicates that the unit will be
# continuing to move at the end of the turn.

extends Node2D

#### Signals

# These signals are given if a mouse enters or exits the unit representative
#
# * is_path - True if the the EOT movement indicator is the source of the signal, otherwise false
# <<<<
signal mouse_entered_item(is_path)
signal mouse_exited_item(is_path)
# >>>>

#### Variables

# Children nodes
onready var _move_ind_node = get_node('MoveIndicator')

#### Private Functions

# Node function, called once all children are ready
func _ready():
	get_node('BodyArea').connect('mouse_entered', self, '_rep_mouse_action', [true, false])
	get_node('BodyArea').connect('mouse_exited', self, '_rep_mouse_action', [false, false])
	get_node('MoveIndicator/PathArea').connect('mouse_entered', self, '_rep_mouse_action', [true, true])
	get_node('MoveIndicator/PathArea').connect('mouse_exited', self, '_rep_mouse_action', [false, true])

# Emit a mouse action depending upon parameters
#
# * is_enter - (Bool) If true emit a mouse_entered_item signal, else emit mouse_exited_item
# * is_path - (Bool) What to give as the is_path signal parameter
func _rep_mouse_action(is_enter, is_path):
	if is_enter:
		emit_signal('mouse_entered_item', is_path)
	else:
		emit_signal('mouse_exited_item', is_path)

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
