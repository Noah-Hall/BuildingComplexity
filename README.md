# BuildingComplexity
This project uses a modular "Lego" system to build floorplans.
Smart Agents can be used to explore and record information about given floorplans.
This exploration is done with the goal of determining the EASE (Evacuation with Acceptable Simplicity in Emergencies).

Smart Agents Priority List: 
    - Search for closest Exit in LOS within 50 meters
    - Search for closest unvisited Door in LOS within 50 meters
    - Search for closest visited Door in LOS within 50 meters
    - Wander to randomly generated points within 50 meters on the floorplan
This means that a Smart Agent will 
    - Ignore a close Door in order to reach a further Exit
    - Ignore a close visited Door to reach a futher unvisited Door
    - Ignore the directives to wander unless there are no Doors or Exits in LOS

Current Concerns:
    - Edge case where there is a deadend with doors and an elbow hallway leading to the Exit such that the Agents will never reach the Exit
    - Edge case where if an Agent enters a room with mulitple visited doors, it may stick to the closest visited door forever