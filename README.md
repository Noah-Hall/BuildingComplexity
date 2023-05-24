# BuildingComplexity

This project uses a modular "Lego" system to build floorplans.
**Smart Agents** can be used to explore and record information about given floorplans.
This exploration is done with the goal of determining the ***EASE*** (Evacuation with Acceptable Simplicity in Emergencies).

## **Smart Agents** Priority List: 
- Search for closest **Exit** in LOS within 50 meters
- Search for closest unvisited **Door** in LOS within 50 meters
- Search for closest visited **Door** in LOS within 50 meters
- Search for closets unvisited **Node** in LOS within 50 meters
- Search for closets visited **Node** in LOS within 50 meters
### This means that a **Smart Agent** will 
- Ignore a close **Door** in order to reach a further **Exit**
- Ignore a close visited **Door** to reach a futher unvisited **Door**
- Ignore a close **Node** in order to reach a further **Door**
- Ignore a close visited **Node** to reach a further unvisited **Node**

## Current Concerns:
- Edge case where there is a deadend with **Doors** and an elbow hallway leading to the **Exit** such that the **Smart Agents** will never reach the **Exit**
- Edge case where if a **Smart Agent** enters a room with mulitple visited **Doors**, it may stick to the closest visited **Door** forever