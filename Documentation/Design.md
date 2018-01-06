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
1. Army Selection - player knows selected enemy faction
1. Deployment - player knows about most of the enemy army, terrain, and what the win conditions are
1. Council of War - players knows non-hidden enemy deployment and can perform last minute changes
1. Battle commences, consists of turns
1. Battle ends once a player meets the win conditions, or after a set number of turns
 
A turn within a battle has the following flow:

1. Give commands to units
1. Click to complete turn
1. Wait for opponent to do the same
1. Review the provided turn resolution
1. Continue to next turn

## Development Stages

### Stage 1

This is the proposed first implementation.
The game will have a basic flow, it starts mid battle and commands can be given, no enemeies.
You can export commands to simulation friendly format.
The simulator simply converts input to output, which can be displayed as a resolution.

Flow:

1. Give commands to units
1. Click resolve, export to sim. and import result
1. Display resolution
1. Repeat

Units exist, have hard-coded or default stats.

Stats:

* Max/Min speed
* Max Acceleration
* Turning Radius v Speed
* Min turn radius

You can give march commands, display expected movement in time.
You can scrub through time.

## Requirements

1. It is multiplayer, 1 vs 1
1. It is a simultaneous turn strategy game
1. Battle-field has terrain
1. Units have predictable autonomy
1. Units have counter-units
1. Range attack viability affected by terrain, range, and other units
1. Morale is a major component
1. Fresh troops are more effective than tired troops
1. Units will try and disengage after critical exhaustion
1. Commands are given no a unit basis
1. Opposing armies will have a predictable composition
1. Match begins with army deployment
1. Council of War held after all players deploy
1. Battle starts after council of war
1. Narrative setting determines available factions
1. Fantasy, ancient, to medieval narrative settings will all be supported
1. Facing of a unit has strategic importance
1. Factions, army-lists and units are configurable
1. Narrative setting determines factions, factions determine unit descriptions
1. Matches occur within a single narrative setting

## Implementation Details


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

> R = turn radius from center
> D = distance from center to edge
> V_1 = outer edge speed
> V_2 = inner edge speed
> V_1, V_2 >= 0, except when rotating, restricts mobility, too complicated otherwise
> V_1, V_2 <= V_max
> V_1 - V_2 <= V_shear, turning is slower than a straight line
> V_1 - V_2 = turn shear, similar/related to turning radius
> V = (V_1 + V_2)/2
> R(V_1^-1 - V_2^-1) = D(V_1^-1 + V_2^-1)

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
