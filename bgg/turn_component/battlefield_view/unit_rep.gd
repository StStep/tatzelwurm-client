# unit_rep.gd
#
# This Scene renders a unit in the battlefield view
#
# The End-of-Turn (EOT) Move Indictor indicates that the unit will be
# continuing to move at the end of the turn.

extends Node2D

#### Constants

# Highlight color options
# <<<<
const COLOR_NONE = Color('#ffffff')
const COLOR_FOCUS= Color('#eef442')
const COLOR_INVALID = Color('#e2342b')
const COLOR_INACTIVE = Color('#b2b2b2')
# >>>>

#### Signals

# These signals are given if a mouse enters or exits the unit representative
#
# * is_path - True if the the EOT movement indicator is the source of the signal, otherwise false
# <<<<
signal mouse_entered_item(is_path)
signal mouse_exited_item(is_path)
# >>>>

#### Variables

#### Private Functions

# Node function, called once all children are ready
func _ready():
	$BodyArea.connect('mouse_entered', self, '_rep_mouse_action', [true, false])
	$BodyArea.connect('mouse_exited', self, '_rep_mouse_action', [false, false])
	$MoveIndicator/PathArea.connect('mouse_entered', self, '_rep_mouse_action', [true, true])
	$MoveIndicator/PathArea.connect('mouse_exited', self, '_rep_mouse_action', [false, true])

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
	$MoveIndicator.visible = en

# Change the position and rotation of the move indicator to the given global values
#
# * gpos - (Vector2) The global position to place the move indicator at
# * grot - (Vector2) The global rotation to give the move indicator
func place_move_ind(gpos, grot):
	$MoveIndicator.global_position = gpos
	$MoveIndicator.global_rotation = grot

# Highlight the body for a given type
#
# * type - (String) The type of highlight being requested [None, Focus, Invalid, Inactive]
func highlight_body(type):
	match type:
		'None':
			$Sprite.modulate = COLOR_NONE
		'Focus':
			$Sprite.modulate = COLOR_FOCUS
		'Invalid':
			$Sprite.modulate = COLOR_INVALID
		'Inactive':
			$Sprite.modulate = COLOR_INACTIVE
		_:
			print('WARNING: Unknown highlight type %s' % [type])

# Highlight the path for a given type and coverage
#
# * type - (String) The type of highlight being requested [None, Focus, Invalid, Inactive]
# * coverage - (Vector2) The portion of the path to cover, values should be between 0 and 1 and ascending
func highlight_path(type, coverage):
	print('Debug: highlight_path not implemented')
