# Project _NAME_

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

_REPLACE OR REMOVE EVERYTING BETWEEN "\_"_

### Student Info

-   Name: VINCENT LE
-   Section: 05

## Simulation Design

_Shepherd is a simulator where you play as a sheep herder, and go through multiple levels of simulating how to navigate a herd of sheep into a single
fenced area. With the limited resources you have, use whatever you can, such as Dogs and Food, to chase the sheep into the pen._

### Controls

-   _List all of the actions the player can have in your simulation_
-   Spawn Marker
    -   UI Input from a Button using the Mouse
    -   Creates a Marker that will show the Dog where to move towards
-   Spawn Dog
    -   UI Input from a Button using the Mouse
    -   Creates a Dog agent that sheep will flee from
    -   The Dog will be attracted to the nearest Marker
-   Introduce Food
    -   UI Input from a Button using the Mouse
    -   Creates a Food agent that will attract the Sheep

## _Sheep_

_The sheep is one of the main agents in this game, and definitely one of the more plentiful. The sheep's function is to be lead towards the gate,
which, will in turn, result in a victory of the game. The sheep will flee from Dogs and be attracted to Food._

### _State 1 Name_

**Objective:** _A brief explanation of this state's objective._

#### Steering Behaviors

- _List all behaviors used by this state_
   - _If behavior has input data list it here_
   - _eg, Flee - nearest Agent2_
- Obstacles - _List all obstacle types this state avoids_
- Seperation - _List all agents this state seperates from_
   
#### State Transistions

- _List all the ways this agent can transition to this state_
   - _eg, When this agent gets within range of Agent2_
   - _eg, When this agent has reached target of State2_
   
### _State 2 Name_

**Objective:** _A brief explanation of this state's objective._

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles - _List all obstacle types this state avoids_
- Seperation - _List all agents this state seperates from_
   
#### State Transistions

- _List all the ways this agent can transition to this state_

## _Dog_

_The dog is an agent that will move in a direction that is straight to the closest Marker. If there are any sheep within that distance,
they will flee from the Dog. However, the Dog will stop before it touches any sheep._

### _State 1 Name_

**Objective:** _A brief explanation of this state's objective._

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles - _List all obstacle types this state avoids_
- Seperation - _List all agents this state seperates from_
   
#### State Transistions

- _List all the ways this agent can transition to this state_
   
### _State 2 Name_

**Objective:** _A brief explanation of this state's objective._

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles - _List all obstacle types this state avoids_
- Seperation - _List all agents this state seperates from_
   
#### State Transistions

- _List all the ways this agent can transition to this state_

## Sources

-   _List all project sources here –models, textures, sound clips, assets, etc._
-   _If an asset is from the Unity store, include a link to the page and the author’s name_

## Make it Your Own

- _Added a third agent: Food, which will be used to attract sheep_
- _Create multiple levels so that there is a "completion" to the game_

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_

### Requirements not completed

_If you did not complete a project requirement, notate that here_

