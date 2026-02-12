# In Bloom

## Trailer

<video controls>
    <source src="videos/in_bloom.mp4" type="video/mp4">
</video>

## What is In Bloom

In Bloom was intended to be a survival horror game where nature has taken over

## My Experience

Not one of my favorite projects

The end result looks quite good but I did not like it either as a concept or as a game.

The game was fine to implement nothing major technical issues I could have really prevented,
we did however have issues communicating ideas during this project, partially due to people missing
and just information lost between disciplines.

We used Fmod and suffered from a lot of issues on that front to begin with.

Unreal had some issues when we were using it, like a example would be how when Perforce was down 
Unreal would also freeze until Perforce was back, though that is more of a Perforce issue than a Unreal one.
The issues we encountered with Unreal was mostly that it was not really that intuitive, like 
why is it (x, y, z) but also (x, z, y) when going from C++ to blueprints


### My Takeaways
- Reinforced that communication is key
- Fmod is a pain
- Unreal was surprisingly annoying to work in

#### Systems I worked on
- Interaction System
- Inventory System
- Doors
- Lighting Transitions
- Audio Triggers
- AOE Effects
- Locks
- Animation Triggers

#### Spawn Safety
We had a issue at a late point in development where the player could move through a wall or floor while loading in,
so I was tasked with solving it as quick as possible and did that by disabling input and gravity until the floor had 
loaded in. Doing that worked surprisingly well and was later tied into the loading screen as the thing to announce
that the level should be loaded now.

We also used a similar thing to fix enemies falling through the map sometimes depending on load order.

<div class="blueprint_image">
<img src="Images/Blueprints/in_bloom/blueprintTest.webp"></img>
</div>

#### Interaction
Here is a blueprint of how the interaction system was implemented,
we had an array of all interaction objects within a area and it would then go over them
and performs the different interaction types on them. Now I would not say I'm happy with how I implemented this
as I could have made it much simpler and easier to use. As a example I would probably not have multiple 
functions on the interface and instead just return a "tag" value and then the number value array.

<div class="blueprint_image">
<img src="Images/Blueprints/in_bloom/Interactor.webp"></img>
</div>

#### Door Code

The door script was one of the first things I made in this project and I ended up 
making it in **C++** since I thought it would be hard to migrate blueprints to **C++**.

Ended up finding out that that was not the case after some time

<pre>
    <code class="language-cpp">
    
UENUM(BlueprintType)
enum class EDoorOpeningMode : uint8 {
	Single,
	Multiple
};

USTRUCT(BlueprintType)
struct FDoorStruct{
	GENERATED_BODY()
	
	FRotator origin = FRotator();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Opening Settings")
	TObjectPtr &lt;UStaticMeshComponent&gt; meshDoor;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Opening Settings")
	float rotation;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Opening Settings")
	float speed;
};

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FDoorOpen);

UCLASS()
class THEBEAUTIFULHORROR_API ADoor : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ADoor();
	UPROPERTY(BlueprintAssignable, Category= "Door")
	FDoorOpen doorOpened;
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	UFUNCTION(BlueprintCallable)
	void ChangeDoorState();

	UFUNCTION(BlueprintCallable)
	void DoorStateFromLocation(FVector location);

	UFUNCTION(BlueprintCallable)
	void AddDoorToOpen(FDoorStruct door);

protected:

	bool isOpen = false;

	const float MIN_LERP = -1;
	const float MAX_LERP = 1;

	float lerpValue = 0;

	// UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Opening Settings")
	TArray &lt;FDoorStruct&gt; rotationPoints;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Opening Settings")
	EDoorOpeningMode doorOpenMode = EDoorOpeningMode::Single;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Opening Settings")
	float rotation = 90;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Opening Settings")
	float openingSpeed = 1;

	float targetRotation = rotation;

	FRotator origin = FRotator();

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	void MoveDoors();

	void MoveSingleDoor();

};

    </code>
</pre>
