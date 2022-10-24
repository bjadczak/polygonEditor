# polygonEditor

![image](https://user-images.githubusercontent.com/73365815/195675582-0332805e-5b08-4a7e-b1bc-76040a4c4b88.png)


This is a project for *Computer Graphics 2022* course on Computer Science course. Starting this project there were defined crutial functionalities that needed to be implemented into the code, they are:
- Adding, editing and deleting a polygon
- For editing:
  - Moving a point
  - Deleting a point
  - Adding a new point in the middle of an edge
  - Moving an edge
  - Moving a polygon
- Adding special contraints call 'Relations', they are:
  - Keeping two edges parallel
  - Keeping set edge a set length
- Deleting a relation
- Displaying an icoon above an edge with 'Relation'
- Implementing two diffrent alghoritms for drawing straight line
  1. Bresenham's line algorithm
  2. Default line drawing alghoritm

---
## Usage
This program operates in couple of modes, that user can select by clicking with RMB on drawing area and selecting desired mode from context menu.

![image](https://user-images.githubusercontent.com/73365815/195675925-18e4056f-e610-4705-b5b9-4d08fd1871ae.png)

Modes are:
- Adding a point
- Moving a point
- Moving an edge
- Moving a polygon
- Deleting a point
- Adding relation
  - Adding fixed length relation
  - Adding parallelism relation
User can manipulate polygons in each of this modes. Apart from that, user can also choose in contex menu which algoritm of drawing line to use.

---
## Alghorithm of relations
When there is a move on the cavnas that nessesitates fixing of relations application calls function *FixPoligons()* that looks throu all stored relations. For each relation we check all lines that are in relations and try to "*fix*" these lines.

Fixing of line is a process of picking one of end points of line and then trying to move it in 8 possible ways:
1. Left
2. Left-Top
3. Top
4. Right-Top
5. Right
6. Right-Bottom
7. Bottom
8. Left-Bottom

After we have checked all options we pick one that took us the closest to desired configuration of lines.

We continiue to iterate throu relations and lines until we reach "*good enough*" configuration or until we cannot fix configuration. Further more, we do not fix the same relation twice in one iteration of whole cycle of fixing relations.l

## Assumptions
* Alghorithm does not move the point that is being draged
* If Alghorithm cannot get to desired state (either becouse it can't happen or becouse we are holding poitn that should be moved), application will try to fix the state one more time after we stop holding the point.

