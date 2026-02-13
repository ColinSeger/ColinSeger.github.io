# Project Liminal

## Trailer
<video controls>
    <source src="videos/project_liminal.mp4" type="video/mp4">
</video>

## What Is Project Liminal

Project Liminal is a school project that was intended to be a game that is re-playable.
The way we tried to make that was via a friend-slop game where the goal is to under pressure identify anomalies.
The way these anomalies reveal themselves is that you and your friend will not be seeing the same thing. 



#### Visual Example:
<div class="image_comparison">
    <img src="../../Images/LegacyImages/snow1.jpg" class="game_image"></img>
    <img src="../../Images/LegacyImages/snow2.jpg" class="game_image"></img>
</div>

In this example a randomly selected player might see the first or second image displayed on a painting in the game.
While the others see the other example.

This project was done 100% in blueprints due to the extreme time constraints


### My Takeaways
- Syncing Multiplayer Is Hard
- In A Small Team Vibes Can Carry You To Success
- Unreal Had Some Weird Issues
<!--That You Would Not Expect From Such A Big Engine -->

#### Systems I worked on

- Anomalies/Syncing Diffrent Visuals
- Triggers  
- Win Condition/Score
- Enemy AI/Behavior
- Optimisation
<!--- The Trigger For Timer And Music-->
<!--- Ending Trigger-->

#### Anomaly Systems

The way it ended up working is overly convoluted since it was our first time doing multiplayer and we were trying to do something 
that is working against what UE5 tries to do when syncing objects online, since we wanted the paintings to be synced in some ways but not all ways
I had to work around the included syncing.

The way it works is that it first spawns the painting picks a random mode which could be visible anomaly for 1 person, visible anomaly for all except 1,
no anomaly and finally one where it picks a anomaly state that is synced for all players meaning it is not a anomaly. After selecting mode it picks a
random painting and if anomaly selects a variations of that painting on the clients it should be anomaly on.


### Triggers

I was in charge of implementing triggers for things like music timer and endings, and also making sure the triggers are synced for both players.


### Optimisation

- DirectX 11
- SM5
- Disabled Nanite
- Added Lods To Some Models

During this project I was actually allowed to do one of my favorite tasks, optimising. Granted I was only allowed one day to do it
but that was plenty of time to pick those low hanging fruits that **Unreal Engine** has. Some of the steps I took was to disable Nanite
since our project mostly had low poly assets, moved back to **DirectX 11** and **SM5** shaders since we where not using features that those could not provide.

Now saying I did that says nothing on how it affected performance of our game, well we whent from 120 fps on our main system to over 300+fps
stable on our main system. You might wonder why I was even given a day to Optimise our game, well it came down to that one of our playtesters
who had a more average rig than us ran at ~20fps unstable before the quick optimisations. After the optimisations we had no performance complaints, 
sadly we did not messure the difference on their rig.

We did move to **DirectX 11** wich I have to admit we did not check the difference of doing that, and the difference was probably minimal but since it is
a older and more esthablished API it should in worst case improve performance only on older Gpu's while sligly lowering performance on newer ones.
But moving to **DirectX 11** pawed the road to the biggest gain **SM5** wich is a simpler shadermodel and while **SM6** should be faster on Cpu heavy games
due to it's multithreaded model our game was not Cpu bottlenecked so we moved to **SM5** and saw a great increase in performance. This is probably due to **SM5**
needing less instructions for materials.

Another low hanging fruit was LODs, since we where not using Nanite on most models we disabled it and made Lods for the models that needed it the most.
