# Procedural Dungeon Generator

##### Table of Contents  
* [Description](#description)
* [Algorithm](#algorithm)
* [Articles](#articles)

## Description

A simple dungeon generator that creates differently sized rooms with hallways connecting them

## Algorithm

First, randomly generated rooms are created. They vary in size and location but all reside in an area close to eachother

![image](https://raw.githubusercontent.com/liviusgrosu/unity-procedural-maze-generator/main/Pictures/random%20generated%20rooms.PNG)

Next, hallways need to be generated in order to connect the rooms. One way we can generate it is by creating a Delauney trianglulation which is a collecton of points and conneting edges that avoids narrow triangles and produces nice looking mesh. Here we are translating the vertices of our triangluation as the centre of rooms and edges as are intended hallway paths. The Bowyer-Watson algorithm was used to generate the triangulation.

![image](https://raw.githubusercontent.com/liviusgrosu/unity-procedural-maze-generator/main/Pictures/Calculated%20Delaunay%20Triangulation.PNG)

To make the dungeon more natural the generator won't be using all the available edges, instead only a select few are choosen. However all rooms must be available for traversal so a minimum spanning tree (MST) is generated such that each room is connected in some way. Prims algorithm was used to generate the MST

![image](https://raw.githubusercontent.com/liviusgrosu/unity-procedural-maze-generator/main/Pictures/Calculated%20MST.PNG)

Furthering the natural look of the dungeon, randomly selected edges from the triangulation set was pushed into the MST to create loops

![image](https://raw.githubusercontent.com/liviusgrosu/unity-procedural-maze-generator/main/Pictures/Random%20hallways%20added%20to%20MSt.PNG)

Then the AStar algorithm was used to translate MST edges to tiles. 

![image](https://raw.githubusercontent.com/liviusgrosu/unity-procedural-maze-generator/main/Pictures/AStar%20generated%20hallways.PNG)

To spruce it up further, custom models where used replacing the plain debug objects

![image](https://raw.githubusercontent.com/liviusgrosu/unity-procedural-maze-generator/main/Pictures/Custom%20Models.PNG)

Finally, randomly generated lighting was added which are spawened evenly from eachother

![image](https://raw.githubusercontent.com/liviusgrosu/unity-procedural-maze-generator/main/Pictures/Light%20Source.PNG)

## Articles

https://stackoverflow.com/questions/39984709/how-can-i-check-wether-a-point-is-inside-the-circumcircle-of-3-points

https://titouant.github.io/testTriangle/

https://www.youtube.com/watch?v=rBY2Dzej03A

https://math.stackexchange.com/questions/978642/how-to-sort-vertices-of-a-polygon-in-counter-clockwise-order

https://www.youtube.com/watch?v=cplfcGZmX7I

https://en.wikipedia.org/wiki/A*_search_algorithm

https://codereview.stackexchange.com/questions/110621/dictionary-getvalueordefault/201182#201182
