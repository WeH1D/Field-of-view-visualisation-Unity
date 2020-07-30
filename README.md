# Field of view visualisation
  Visualisation of a filed of view (FOV) in a top down scenario using rays.

## Description
  This project came to mind stumbling upon an article (https://www.redblobgames.com/articles/visibility/) describing how to achieve field of vision in a top down map. I found this article fun and interesting to read so I decided to have a bit of fun and try and apply the methods author described to achieve a similar effect. The project was done in Unity.
  
## Bacis priciples
  To achieve this effect the author describes couple of methods:
* #### Casting rays in all directions arond the actor
  This is also the simplest method and involves just casting certain number of rays in all directions from the center.
  The space in-between the rays is then filled in.
  
  ![image](https://github.com/WeH1D/Field-of-view-visualisation/blob/master/images/method%201.png)
* ### Casting rays only towards ends of walls
  This method uses a significantly smaller number of rays to determine the field of view of an actor. Its also a method that is less jittery when the actor moves around.
  I achieved this method a bit differently then the author described, but the core principle is still there.
  In my case I just needed to sort the rays by angle before having to fill them in. 
  
  ![image](https://github.com/WeH1D/Field-of-view-visualisation/blob/master/images/method%202.png)

## Notes
  * The code is probably far from perfect or optimised, but this was a fun side project I did to keep myself from getting bored during the summer break and so my goal wasn't to write the most optiomised code possible. 
  * The process of filling in the space between rays was done by generating a mesh of triangles with endpoints in the ray hit points. There deffinetly has to be a better way of doing this as this method produces some janky results when visualising the field of vision. 
