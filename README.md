# SHEPHERD

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

### Student Info

-   Name: VINCENT LE
-   Section: 05

## Simulation Design

_Shepherd is a simulator where you play as a sheep herder, and go through multiple levels of simulating how to navigate a herd of sheep into a single
fenced area. With the limited resources you have, use whatever you can, such as Dogs, Food, and Markers, to chase the sheep into the pen._

### Controls

-   _Spawn Marker_
    -   _UI Input from a Button using the Mouse_
    -   _Creates a Marker that will show the Dog where to move towards_
-   _Spawn Dog_
    -   _UI Input from a Button using the Mouse_
    -   _Creates a Dog agent that sheep will flee from_
    -   _The Dog will be attracted to the nearest Marker_
-   _Introduce Food_
    -   _UI Input from a Button using the Mouse_
    -   _Creates a Food agent that will attract the Sheep_

## _Sheep_

_The sheep is one of the main agents in this game, and definitely one of the more plentiful. The sheep's function is to be lead towards the gate,
which, will in turn, result in a victory of the game. The sheep will flee from Dogs and be attracted to Food._

### _State 1: Wander_

**Objective:** _The sheep will wander, staying together as a pack while also applying separation to not overlap each other._

#### Steering Behaviors

- _Wander, Cohesion, Align, Separation_
   - _StayCohesive - list of sheep_
   - _Align - list of sheep_
   - _Separation - list of sheep_
- Obstacles - _Screen Bounds_
- Separation - _Separates from other Sheep_
   
#### State Transistions

- _When the sheep are currently not fleeing and not seeking something within range_
   - _When this agent is not within range of a Food_
   - _When this agent is not within range of a Dog_
   
### _State 2: Flee_

**Objective:** _Sheep will flee if in range of a Dog._

#### Steering Behaviors

- _Flee, Cohesion, Align, and Separation_
   - _Flee - Dog in range_
   - _StayCohesive - list of sheep_
   - _Align - list of sheep_
   - _Separation - list of sheep_
- Obstacles - _Screen bounds, Dog_
- Seperation - _Separates from other Sheep_
   
#### State Transistions

- _If a Dog is detected within range_

### _State 3: Seek_

**Objective:** _Sheep will seek if in range of a Food._

#### Steering Behaviors

- Seek, Cohesion, Align, and Separation_
   - _Flee - Food in range_
   - _StayCohesive - list of sheep_
   - _Align - list of sheep_
   - _Separation - list of sheep_
- Obstacles - _Screen bounds_
- Seperation - _Separates from other Sheep_
   
#### State Transistions

- _If a Food is detected within range_

## _Dog_

_The dog is an agent that will move in a direction that is straight to the closest Marker. If there are any sheep within that distance,
they will flee from the Dog. However, the Dog will stop before it touches any sheep._

### _State 1: Seek_

**Objective:** _The Dog will seek a marker within range._

#### Steering Behaviors

- _Seek_
   - _Seek - Marker in range_
- Obstacles - _Screen bounds_
- Seperation - _None_
   
#### State Transistions

- _If a Marker is placed within range and there is not an Obstacle in range_
- _If neither an Obstacle or a Marker is in range_
   
### _State 2: Flee_

**Objective:** _Dog's will flee away from fences - higher weight than them seeking Markers._

#### Steering Behaviors

- _Flee - if a fence is in range_
- Obstacles - _Fences_
- Seperation - _Separates from Sheep_
   
#### State Transistions

- _If an Obstacle is within range_
- _If neither an Obstacle or a Marker is in range_

### _State 3: Freeze_

**Objective:** _Dog's will stop all movement._

#### Steering Behaviors

- Obstacles - _Fences_
- Seperation - _Separates from Sheep_
   
#### State Transistions

- _If an Obstacle is within range_
- _If a Marker is in range and there is not an obstacle in range_

## Sources

-   _https://thkaspar.itch.io/tth-animals_
-   _https://gvituri.itch.io/tiny-ranch_

## Make it Your Own

- _Added obstacles such as Markers and Food that will be used to attract Dogs and Sheep respectively._
- _Create multiple levels so that there is a "completion" to the game_
- _Add a "Goal" that sheep can enter and then be "penned" - allows for not all sheep to have to be in the pen to win, but rather that the herd can be slowly thinned.

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_

### Requirements not completed

_If you did not complete a project requirement, notate that here_

