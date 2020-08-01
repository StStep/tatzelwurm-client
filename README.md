# Codename Tatzelwurm

This is an as-yet unnamed simultaneous turn mass-battle game.
There is no real timeline for this project, given it being a side-project for an otherwise busy singular developer.
The inspiration for this game is a love for mass-battle miniature games and the Total War franchise, combined with a frustration for the amount of fast-paced RTS skills required for playing Total War multiplayer games.

The game engine Godot is being used, with a majority of the source being in C#.
The primary app is in the directory `./bgg`.
See the GitHub [wiki](https://github.com/StStep/tatzelwurm-client/wiki) and issues (using ZenHub) for some context on where it is headed.

## Requirements

* [Godot Game Engine](https://godotengine.org/) - The most recent tested version is `3.2.2`

## Known Issues

* Case sensitive platforms, such as linux, will have issues with some of the project linking
* `./UnitTests` implementation is broken due to recent design changes
