# Pong2D
Prototype of a simple Pong game.A Hobby project for Gram Games developed in Unity3D 5.0.1.Currently works with Unity Editor and Android 5.0+.

![](/Screenshots/7.png)

All code files are documented for interest
![](/Screenshots/3.png)

##_**Video Preview**_
[Pong2D Video Preview](https://www.youtube.com/watch?v=l6usVKbaBZU&feature=youtu.be "Pong2D Video Preview")

## _**Game Logic Object**_

Game Logic is an empty game object that controls the state of the game and UI.It creates all resources required on the runtime and spawns them when the level is loaded.It checks if a player has scored and restarts the game or finishes the game if either one of the players have the maximum score **const int m_iMaxScore = 5** .When a player scored,a delay has been issued and the game is delayed according to **const int m_iDelayBetweenScores = 3**. We have the **GameState** object which holds the current game status **bool m_bStatus**. If this becomes false, meaning a player has scored or game is over.

### _**Paddle Object**_
Paddle is a prefab which is instantiated by Game Logic twice at the runtime.If AI mode,AIPaddle prefab will be instantiated that consists an other script other than Paddle.This object's job is to track input and control the paddle accordingly.Paddle movement isnt linear so you dont touch/click to go up or down.You can drag your finger across the half right or left side of the screen to drag the paddle with a linear or non linear speed.This is accomplished by controlling a boolean **bool m_bDragging** and getting the delta position _**dv=Touch.deltaPosition**_.

## _**Ai**_
AI prefab will be instantiaed from Game Object at the runtime.There are 5 difficulties **Ai.Difficulty** Easy Normal Hard Unfair and Impossible.These levels are accomplished with a Numerical Spring method as discussed [here](http://allenchou.net/2015/04/game-math-numeric-springing-examples/). I have included low/high Linear Damping and low/normal/high Angular Frequency to illustrate **"awareness"** of AI.Lower difficulties have **low** damping which means they will not catch up to the **Ball** object in time thus it is easy to score.Higher difficulties except Impossible have lower damping and higher AF thus when they're in range with the Ball object they will react faster,their position will close with a higher rate.Impossible difficulty has no use of numerical springing.It is impossible because it copies the ball movement on the Y axis.Therefore it is impossible to score against the highest difficulty **except** the ball goes through the paddle.It can happen on high speeds because the ball can go through the paddle in between two frames.There are quite numerous solutions to this like Discrete/Continuous collision detection but to be honest I didnt bother to include it on the code.**Ai.Difficulty** is being set on the MainMenu.level using **PlayerPrefs** class.

##_**Ball Object**_
This is a straight forward object that only does collision checking with the screen boundaries.Only thing to mention is **Ai.SpeedState** enum which is used in difficulties.Higher difficulties has **Ai.SpeedState.Exponential** which means the ball speed will increase in time.


###Screenshots from Editor
![](/Screenshots/1.png)
-
![](/Screenshots/2.png)
###Screenshots from Nexus 5(Android 5.0.1)
![](/Screenshots/4.png)
![](/Screenshots/5.png)
![](/Screenshots/6.png)
##Changelog
###11.5.2015
- 2 Player Mode
- AI Mode 
- 5 Difficulty level
- Added 3 seconds delay in between scores


###8.5.2015
- Added Main Menu
- Added Pause Menu
- Basic Game Logic
- Input Manager
- Touch and Mouse support
