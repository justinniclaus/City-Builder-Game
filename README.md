# City Builder Game

A 3D city-building simulation game created in Unity. Place roads, houses, and special buildings to grow your city while managing resources and completing goals.

## Features

- **Interactive City Building**: Place roads, houses, and special structures in a grid-based environment
- **Resource Management**: Balance your budget while expanding your city
- **Goal System**: Complete objectives to earn rewards and progress
- **Income Generation**: Buildings generate regular income based on population and building types
- **Road System**: Intelligent road connections that automatically adjust based on adjacent pieces
- **Camera Controls**: Navigate around your city with intuitive camera movement

## Game Mechanics

### Building Types
- **Roads**: Connect different parts of your city. Roads automatically adjust their appearance based on connections.
- **Houses**: Generate population and basic income. Must be placed adjacent to roads.
- **Special Buildings**: Cost more but provide significant income and population boosts. Must be placed adjacent to roads.

### Resource Management
- **Money**: Start with a set amount and earn more through income generation
- **Population**: Grows as you place houses and special buildings
- **Income**: Generated regularly based on your population and building count

### Goals System
The game includes progressive goals to achieve:
1. Initial goal: Reach 20 population and 1 special building
2. Middle goal: Reach 50 population and 2 special buildings
3. Final goal: Reach 100 population and 4 special buildings

Each completed goal rewards you with additional funds to expand your city further.

## Controls

- **Camera Movement**: Use arrow keys or WASD to move the camera around the map
- **Building Placement**: Click the respective UI button to select a building type, then click on the map to place it
- **Deletion**: Select the delete tool and click on buildings or roads to remove them

## Technical Implementation

The game is built in Unity and makes use of several systems:

- **Grid-Based Placement**: All structures are placed on a grid system
- **Event-Driven Architecture**: Components communicate through events and actions
- **Singleton Pattern**: Used for managers and global access points
- **Object Pooling**: Efficient resource management for building placement
- **A* Pathfinding**: Used for road placement between two points
- **Weighted Random Selection**: For variety in building appearances

## Project Structure

The codebase is organized into several manager classes:

- **GameManager**: Core game loop and coordination
- **CityManager**: Handles city statistics, goals, and economy
- **StructureManager**: Manages building placement and validation
- **RoadManager**: Handles road placement, path finding, and connections
- **PlacementManager**: Controls the actual placement of objects on the grid
- **InputManager**: Processes player input and translates to in-game actions
- **UIController**: Manages UI elements and button interactions

## Credits

Created by [Your Name] for 3D Programming Class

Built using Unity and C#

Road system based on techniques from Sunny Valley Studio
