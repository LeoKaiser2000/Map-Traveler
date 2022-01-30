<h1 align='center'>
<img src='/ReadmeAssets/Scheme.png' alt='Map-Traveler'/>
</h1>

<h3 align='center'>
Unity map traveler for school
</h3>

<p align='center'>
<img src='/ReadmeAssets/Screenshot.png'/>
</p>

# Summary
* [Requirements](#requirements)
* [Quickstart](#quickstart)
* [Project overview](#projectOverview)
* [Project features](#projectFeatures)
* [Remarks](#remarks)


## <a name='requirements'>Requirements</a>
C# version 9.0

*Note: This is the version that I used at default, it may run on lower one.*

## <a name='quickstart'>Quickstart</a>

command:

```bash
$ git clone "https://github.com/LeoKaiser2000/Map-Traveler.git"
```
Clone the project, build sln with your favorite compiler or IDE.

## <a name='projectOverview'>Project overview</a>

This project is a map traveler unity project for school.
The goal is to train with threads and couroutines.

The game is separated in 4 state:
```
    PlayerInput
    PlayerPathFinding
    PlayerMove
    FoeMove
```

### Player Input

This phase is the phase where player can select is move or create/remove foes.

Move request is made with left mouse clic on a cell.

Foe creation/deletion is made with right mouse clic.

### Player Path Finding

When the player request a move, the game state become PlayerPathFinding.

In this phase, the player path is check in another thread.

If any path is found, the gstate back to PlayerInput.

### Player Move

After found a valid path, player move to it.

### Foe Move

The last action is the move of all foes. This move are made in a coroutine.

The foes move in an empty adjacent place in 1s, with a wait of 0.5s within 2 moves.


### Threaded Request

Thread Request class handle return value like Task, but in a different thread.

## <a name='projectFeatures'>Project features</a>

### Mandatory

* Map Ganeration (0.5 points)

* Player Path search (2 points)

* Player Move (0.5 point)

* Foes Move (1 point)

### Bonus

#### Player Path Move

Player follow the found path, instead of ghost traveling.

#### Foe Creation/Deletion

Foes can be add or remove with the right mouse clic to ease game testing.


#### Random Generation

To test a maximum of configurations, map is generated randomely.

## <a name='remarks'>Remarks</a>

I didn't know how to handle foe movement considering adjacent characters. My foes c'ant go on a cell where another foe or the player already is. If a foe can't move (all adjacent cells are full) we will keep is postion instead of moving.

Use the right clic to test different configurations easier.

I made a misatke in my pathfinding recursion and did not success to corect it in time. Some path may be ignore when other are already found and the final path can be not the obtimal.

*Note: I made it on a Linux platform, but check compilation on windows too.*

*If you have any trouble, please contact me.*
