# BuildingComplexity

## Summary
This project uses a video game style "level builder" system to build floorplans.
**Smart Agents** can be used to explore and record information about given floorplans.
This exploration is done with the goal of determining the ***EASE Score*** (Evacuation with Acceptable Simplicity in Emergencies).

## About the Floorplan Builder
### Object Explenations 
#### (some more in depth info on the code aspect of objects is seen below under **Floorplan** Info)
- **Floor**: where the agents "walk" to traverse the floorplan.
- **Wall**: represents the walls of a building which agents cannot traverse through.
- **Room Node**: where the agents "spawn" in the floorplan.
- **Intersection Node**: should be placed on natural intersections between hallways within the floorplan.
- **Module Node**: should be placed anywhere where there may not be sufficent alternative **Targets** for the agents to see for traversal.
- **Stair**: act as "portals" to other **Stair** objects in the same stairwell.
- **Door**: acts as a door between rooms, hallways, etc.
- **Exit**: acts as the exit to a building. This is an agents ultimate goal.
### Floorplan Requirements
- Floorplans must have at least one **Room Node** (otherwise, no agents would spawn).
- Floorplans must have at least one floor square **outside** of an **Exit** (this is due to how NavMesh generates surfaces; agents will not be able to reach an **Exit** which does not have a floor on the other side).
- All **Room Node** objects must have at least one valid path to an **Exit**.

## About the Floorplan Runner
- The Floorplan Runner only has a speed selector slider and "run" button.
- Current undesired behavior where running a floorplan multiple times may cause crash. Best to exit and reload floorplans for testing multiple runs.

## **Smart Agents** Info:
### Target Priority List (priority always to least visited when applicable)
- Search for closest **EXIT** in LOS within 50 meters
- Search for closest **STAIR** in LOS within 50 meters (only if agent is not on exit floor)
- Search for closest **UNVISITED INTERSECTION NODE** in LOS within 50 meters
- Search for closest **UNVISITED DOOR** in LOS within 50 meters
- Search for closest **UNVISITED NODE** in LOS within 50 meters
- Search for closest **VISITED DOOR** in LOS within 50 meters
- Search for closest **VISITED NODE** in LOS within 50 meters
### Specific Behavior 
- Ignore a close **DOOR** to reach a further **UNVISITED INTERSECTION NODE**
- If an **INTERSECTION NODE** is visited, it gets treated as a regular **VISITED NODE**
- Ignore any close more visited **TARGET** to reach a further less visited **TARGET** of the same type
### Tracked Data
#### Solely During Simulation
- Dictionary of all **Targets** that the **Smart Agent** visits: **visitedTargets**
- Current **Target** GameObject: **targetObject**
- Current floor of **Smart Agent**: **_currentFloor**
- Position of **Smart Agent** from previous Update() call (used to calculate **totalDistanceTraveled**): **prevPosition**
#### Post Simulation
- Distance between **Smart Agent** start-position and end-position: **lineToExit**
- Total distance traveled by **Smart Agent**: **totalDistanceTraveled**
- Number of **Targets** visited by **Smart Agent**: **numVisitedTargets**

## **Floorplan** Info:
**Floorplans** are made up of floors which are layed out horizontally rather than vertically (similar to a blueprint).
### Components
- **Floors**: the GameObjects that makeup the floor which **Smart Agents** walk on
- **Borders**: the GameObjects used to border the **Floorplan** (essentially **Floors** that do not interfere with the square meter calculations)
- **Walls**: the GameObjects that makeup the obsticles of the **Floorplan**
- **Doors** (**Target**): the GameObjects that **Smart Agents** pass through to go from room to room
- **Stairs** (**Target**): the GameObjects that **Smart Agents** use to go from floor to floor
- **Exits** (**Target**): the GameObjects that **Smart Agents** look for to exit the simulation
- **Intersection Node** (**Target**): the invisible GameObjects that are used to signify an intersection within the **Floorplan**
- **Room Node** (**Target**): the invisible GameObjects that are used to signify the center of a room within the **Floorplan**
- **Module Node** (**Target**): the invisible GameObjects that are used to signify the center of a module within the **Floorplan**

## **Stair** and **Stairwell** Info:
### **Stair** GameObjects and **StairScript**
**Stair** GameObjects represent stairs accessible from a given floor
#### Attributes
- **_floor** (int): used to keep track of which floor the **Stair** is on
- **_stairwell** (int): used to keep track of which **Stairwell** object the **Stair** belongs to
- **isExitFloor** (bool): true only if **Stair** is on the same floor as an **Exit**
### **Stairwell**
A **Stairwell** object represents a staircase by pulling related **Stairs** into a list to form a functional staircase
#### Attributes
- **_stairwell** (List): a list of **Stair** GameObjects which forms the actual staircase
- **_num** (int): a unique id that identifies a **Stairwell**, it should match the **Stair**.**_stairwell** of each **Stair** in **_stairwell**
- **_count** (int): the number of **Stairs** in **_stairwell**
- **_exitFound** (bool): set to true if **_stairwell** has a **Stair** on a floor with an **Exit**
- **_exitFloor** (GameObject): stores the **Stair** on the floor with an **Exit** if there is one