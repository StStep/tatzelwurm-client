# Design Notes

## Summary

Code name Tatzelwurm is a simultaneous-turn mass-battle game.

This game targets the following ideas:

* Strategy over tactics
* Feel like planning out a battle
* Units have predictable movement
* Asymmetrical army composition
* Ability to learn and expect movement patterns and actions but still allowing surprise

### Game Flow

A match has the following flow:

1. Faction selection - player knows possible enemy factions
2. Army Selection - player knows selected enemy faction
3. Deployment - player knows about most of the enemy army, terrain, and what the win conditions are
4. Council of War - players knows non-hidden enemy deployment and can perform last minute changes
5. Battle commences, consists of turns
6. Battle ends once a player meets the win conditions, or after a set number of turns

A turn within a battle has the following flow:

1. Give commands to units
2. Click to complete turn
3. Wait for opponent to do the same
4. Review the provided turn resolution
5. Continue to next turn

### Client and Server

The game is split into two applications: client and server.
Two separate clients connect to a common single server to play a match.
The server application contains the simulation process and match arbitration.
The client application contains the user interfaces for match creation, command input, and viewing the turn resolutions.

## Requirements

1. It is a simultaneous turn strategy game

    1. The focus is on 1v1 multiplayer
    2. Each player controls an army made of units
    3. About a 30 min estimated match-time for knowledgeable and available players
    4. It is top-down, with 2D graphics
    5. There is not perfect information for either player
    6. Commands are given on a unit basis
    7. Player will not know the win conditions when making an army
    8. Each player knows the win objectives during a match
    9. A match has a set maximum number of turns
    10. The game has simultaneous turn resolution

        1. The players submit orders for their turn, which are sent to a server for resolution
        2. The server provides the resolution to both players once they have both submitted orders
        3. The next turn cannot be planned until the resolution for the previous turn has been generated
        4. The match can be played asynchronously, and a player can have multiple ongoing matches
        5. A max time for order submission can be selected, where it generates a resolution despite missing orders

2. Predictability with surprise

    1. Enemy composition is predictable

        1. The narrative setting determines the available factions
        2. Each faction provides an armylist of possible units
        3. An army has a faction and is made of units chosen from the faction armylist
        4. Player has a chosen faction and knows the enemy faction when creating an army

    2. Enemy movement predictable, know how units can move in most cases
    3. Unit match-up winners are predictable, but not guaranteed if close in power
    4. Owned unit reactions predictable, know how a unit will respond to an unexpected charge

3. Battle-field has terrain

    1. Terrain affects movement
    2. Terrain affects ranged attacks
    3. Different units can move through easier or ignore some terrain altogether
    4. There is beneficial terrain, choke-points, hills, etc

4. Army creation is important to strategy

    1. Units have hard and soft counter-units
    2. Certain units will be limited in the number that can be brought
    3. Certain units will be required to create a base for the army

5. Morale is important to strategy

    1. Units will have varying leadership levels
    2. Low quality troops will more likely flee when the situation is unfavorable
    3. Leadership units can counteract low leadership levels
    4. Loss of leadership hurts morale
    5. A retreat due to low morale is more common than all unit members being killed

6. Exhaustion is important to strategy

    1. Soft stamina acts as a balance for short-term exertion, especially in relation to movement speed
    2. Hard stamina balances overall usage of units, range from fresh to exhausted
    3. Fresh troops are more effective than exhausted troops
    4. Units will try and disengage after critical exhaustion

7. Location is important to strategy

    1. Facing of a unit has strategic importance

8. The available units are configurable

    1. The available narrative settings, factions, army-lists and units are configurable
    2. A wide variety of settings, from fantastical to historical narrative settings will be supported
    3. Matches, therefore balancing, will only occur within a single narrative setting
    4. The server enforces the chosen configuration

## Development Stages

### Stage 1 (0.1)

This stage focuses on the initial mid-game UI, unit movement, and simulator hooks.

The game will start mid battle with units on the field.
Only move commands can be given, and there are no enemies.
The turn can be resolved, and the resolution should be displayed.
At this time, the simulator can be very simple, but the import/export formats need a definition.

Units will have hard-coded stats, which include:

* Max/Min speed
* Max Acceleration
* Turning Radius v Speed
* Min turn radius

#### Flow

The player can commands movement to units and see how they move with the resolution.

1. Player clicks on units and creates the commands
2. Player clicks on the resolve button
3. Player can view the resolution
4. Repeat


## Client

The client is the primary interface for the player.

It is separated into the following components:

* Setup Component - This is the interface for negotiating the match details with the server
* Turn Component - This is the interface where the turn orders are created by the user to be sent to the server
* Resolution Component - This is the interface that displays the turn resolutions generated by the server

Each component will be treated separately for development purposes, with defined protocols between them.

### Setup Component

Allows for querying server for configurations and matchmaking.

I/O From Server:

* Available Configurations

I/O To Server:

* Match Setup details


### Turn Component

The turn component provides control and insight into what is going to happen during the upcoming resolution.

Resources Include:

* (External) Initial Conditions
* (External) Game Stats
* Command Queue

The turn component scenes include:

* Main
* Battlefield View
* Command GUI
* Command Validator

Controls (TBD):

* 'End Turn' button
* Turn Overview Control
* Unit Overview Control

##### Turn Overview Control

The turn overview provides context to the battlefield.
It has markers for major events, and can scrub through the expected resolution, and it can be used to preview all battlefield motion at once.
The previews occur as ghosts on the battlefield.
Any commands beyond the 1 turn of resolution are grayed out, and not considered.

A major event would be a timing factor such as a charge or ranged attack command.

##### Unit Overview Control

The unit overview allows for fine-tuning of a unit's command within a turn.
It provides the time perspective to the battlefield's position perspective, and allows for previewing of commands independent of other units.
The preview occurs as ghosts on the battlefield.

Suggested options include:

* Minimize Effort, take longer but use least energy possible
* Minimize Time
* Insert delay between commands


#### Component I/O

I/O From Server:

* Initial turn state

I/O To Server:

* Turn commands

#### Main

The main scene controls all of the others, everyone talks to main and no other scene.
Multiple scenes use the Game Stats resource however.

Does Selection need to be a thing?
What about hover menus?

#### Battlefield View

The battlefield is the primary turn interface, the renderer of the commands, and is the backdrop of the entire turn component.
It also provides mouse-over information.

* Main inserts initial conditions
* Add new commands as they are finalized
* Main controls render logic, highlights, warnings, etc.

##### Display Initial Conditions

Displays the initial conditions for the turn:

* Unit Placement/Base
* Terrain

##### Display Commands

Displays current commands with an interface to add new commands to display

##### Controllable Rendering

Hide, show, highlight, warnings, ghosts

#### Command GUI

Provides icons for command selection and previews movement commands.

1. Main tells it when to show up and where to be and the stat_id to reference.
1. While active, it captures input.
1. Once a command is finalized, it reports it to main.

##### Position Node

Nodes surrounded by icons for adding commands

* Halt - If selected, no further march movement following node, otherwise always going forward as default
* Reposition
* Rotate
* Wheel - two separate icons
* Arc - Two Separate icons
* Future Action - a future action could be shown in a uniform place

##### Move Edge

Has Speed selector

Speed choices limited by start speed, time on edge, move type, and unit stats.

#### Command Validator

1. Main asks it to review command queue, and possible preview command
1. It reports any warnings

### Resolution Component

* Allow for scrubbing
* highlight conflicts
* provide movement overview

I/O From Server:

* Detailed run-time information

I/O To Server:

* N/A

### Commands (Remove)

I want to support common mass-battle tactics.

Include:

* Movement
* Formation modification
* Charge
* Ranged Attack
* Brace
* Disengage, special reposition

#### Movement

Speed is continuous between 0 and a units max velocity.
It is chosen manually or by placment restrictions.
Short term fatigue, like a heat-gauge, forces a penalty on overexertion.

Movement consists of:

* Marching - Straight line or arc
* Reposition - Straight line in any direction, slow
* Wheel - Change frontage faster than rotation but requires more room, one corner is fixed
* Rotation - rotate about center, slower, but takes less room than wheel
* Charging - has to have some level of adaption
  * Choose unit in charge cone? Like hurricane cone
  * Re-targeting with basic rules

Turn angle calculations:

```
R = turn radius from center
D = distance from center to edge
V_1 = outer edge speed
V_2 = inner edge speed
V_1, V_2 >= 0, except when rotating, restricts mobility, too complicated otherwise
V_1, V_2 <= V_max
V_1 - V_2 <= V_shear, turning is slower than a straight line
V_1 - V_2 = turn shear, similar/related to turning radius
V = (V_1 + V_2)/2
R(V_1^-1 - V_2^-1) = D(V_1^-1 + V_2^-1)
```

#### Overrides

Overrides occur when a unit experiences unexpected behavior.
It creates simple unit autonomy.

Overrides occur when:

* Enemy charges
  * Brace, shoot?
  * Counter charge
  * Fallback, shoot?
* Enemy disengages
  * Hold
  * Follow

## Server

This section is mostly conjecture at this point.

The server has the following components:

* Simulator Component - This performs the simulation of turn input and the current state to produce the end-of-turn state and turn resolution.

### Units

#### Members (extra)

While units can effectively be viewed as blocks, separating out soldiers could add to the game.

Thoughts:

* Units fight through unit-members, they have facing, melee-range, and exhaustion
* Act as targets for AoE, spacing as a unit parameter
* Command members represented in unit
* Simulates tiring out units better, outnumbering has an advantage
  * have to rotate members only after minutes of melee combat
* Variation in casualty level and unit consistency
* Spacing would be affected by movement speed, and unit training

Casualties include dead. downed and wounded:
* Dead - drop equipment and get in the way
* Downed - drop equipment, can't move, get in the way and can be killed or recovered, if recovered become wounded instead
* Wounded - can walk self to safety, might drop equipment, can continue fighting with reduced effectiveness

Unit Composition/Description

* Provided by factions
* Includes members, equipment, possible formation, stats, and options
* Can be homogeneous or heterogeneous
* Can be organized or unorganized, facing penalties

Locations

* Rigid tray with embedded members
  * Dead or downed down members? Drop, fix, slow, or move back?
* Squares vs hex tiling. Hex can be denser, should use to denote loose
  * Hex better at movement, Helpful if super huge?
  * Hex is more dense if min spacing is constant

Weapons

* Spears, thin triangles, swords ares less directional
* Turn speed? Makes important, determine multi-rank junk

Spacing

* More space, easier movement, hex specifically adds front space and flanks
* less spaces improved attack coverage, additional defense/attack boost?
* Space choices determined by training? Tight, open, loose?

Direction

Unit without unified direction can't march, only reposition.

#### Logistics (extra)

Want to go above and beyond games like total war, supply-lines should be important.
The goal is make encirclement's more dangerous, provide an advantage to smaller well-supplied troops, and to disadvantage widely space troops.

Thoughts:

* Ammo is finite
* Equipment can be lost or broken
* Units require water
* Supply-lines provide ammo, equipment, and water
* Non-combat support troops are important:
  * Couriers
  * Helpers to remove dead, treat wounded, and clear ground of debris
  * Pack animals and supplies

#### Unit Ideas

* Armored necromancer in back, herding zombie horde, if you kill the necromancer the horde falls apart
* Pavase front-line, pike second, than guns
* Wagon with a bunch of guns firing out the sides

### Combat

Goals:

* avoid hidden stats
* armor
  * Strength helps counter
  * armor piercing is hard counter
* Ray-trace weapons, i.e., cannons
  * deals with friendly fire and makes formation density important
* Actions take time
* AoE, ditto with ray-trace possibility for monsters?
  * Avoidable vs unavoidable, swipe vs cannon
  * Sequential vs simultaneous, cannon vs explosion

Stats:

* Morale, vigor, short-term fatigue
* basic attack/defense stats, %
* vertical and horizontal size classes, S, M, L

Wounding:

* Impacts performance
* Allow for impossible to kill matchups

Ranged Attacks

* Elevation abstraction
* Can volley, certain weapons can area select
* Direct fire

Equipment Ideas

* Shields shouldn't provide much benefit to the already heavily armored
  * Classically, shield would just get in way further reduce mobility
* Ranged weapons
  * crossbows, good range and accuracy, slow fire rate
  * bow, medium range and accuracy, fast fire rate
  * gun, bad range and accuracy, slow but armor piercing
* Ultimately determined by narrative

### Configuration

Allow for large level of configuration through easy-edit files.
More of a base for sim-turn mass battle games.
Want people to write and balance their own narrative settings

Look into allowing game mechanics to be configured:

* turn of fog-o-war or logistics.
* determine stats being used and calculations
* what abilities or descriptions exist
