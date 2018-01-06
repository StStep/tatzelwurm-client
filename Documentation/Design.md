# Design Notes



## Summary

Code name Tatzelwurm is a simultaneous-turn mass-battle game.

This game targets the following ideas:

* Strategy over tactics
* Feel like planning out a battle
* Units have predictable movement
* Asymmetrical army composition
* Ability to learn and expect movement patterns and actions but still allowing surprise

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



## Implementation Details


### Game Flow

How a player experiences the game.

#### Stage 1

The player can commands movement to units and see how they move with the resolution.

1. Player clicks on units and creates the commands
2. Player clicks on the resolve button
3. Player can view the resolution
4. Repeat


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


### Commands

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


### User Interface

Nodes

* Position node - final position of movement
* State node - where state changes

Colors for speed, type, etc.

Markers/pips for speed?

State changes separate layer than position

#### HUD

Turn overview, shoes major events
Provides context

Command Overview, shows unit overtime, allows for task speed/time balance

Resolve button

Combine resolution with future preview

#### Resolution View

* Allow for scrubbing
* highlight conflicts
* provide movement overview


### Configuration

Allow for large level of configuration through easy-edit files.
More of a base for sim-turn mass battle games.
Want people to write and balance their own narrative settings

Look into allowing game mechanics to be configured:

* turn of fog-o-war or logistics.
* determine stats being used and calculations
* what abilities or descriptions exist


### Simulation

#### Client vs Server

Need to determine how export and import to simulation works.
Probably going to start with a converter at first, then later move to a stand-alone application.
I want the server to host most of the logic, client is given just what they need for UI and resolution display.
The server would act as the arbitrator and the limiter of bad-actors.

Server

* Takes simulation input
* Generates resolution
* Sets up the rules, possible units, etc.

Client

* GUI
* Displays current state
* Displays resolution
* Provides controls for generating simulation input
* provides information about server configuration
