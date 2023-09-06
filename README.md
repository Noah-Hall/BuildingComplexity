# BuildingComplexity

This project uses a modular "Lego" system to build floorplans.
**Smart Agents** can be used to explore and record information about given floorplans.
This exploration is done with the goal of determining the ***EASE*** (Evacuation with Acceptable Simplicity in Emergencies).

## **Smart Agents** Info:
### Priority List
- Search for closest **Exit** in LOS within 50 meters
- Search for closest **Stair** in LOS within 50 meters
- Search for closest **Door** in LOS within 50 meters (priority to least visited)
- Search for closest **Node** in LOS within 50 meters (priority to least visited)
### Specific Behavior 
- Ignore a close **Stair**, **Door**, or **Node** in order to reach a further **Exit**
- Ignore a close more-visited **Stair** to reach a further less-visited **Stair**
- Ignore a close **Door** to reach a further **Stair**
- Ignore a close more-visited **Door** to reach a futher less-visited **Door**
- Ignore a close **Node** in order to reach a further **Door**
- Ignore a close more-visited **Node** to reach a further less-visited **Node**

## Current Concerns:
- Edge case where there is a deadend with **Doors** and an elbow hallway leading to the **Exit** such that the **Smart Agents** will never reach the **Exit** (maybe if a door has been visited a certain number of times, have the agent pick less visited nodes... what should the equation for that be?)