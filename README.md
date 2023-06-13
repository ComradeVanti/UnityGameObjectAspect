# Unity GameObject Aspect

Aspect-oriented programming for Unity GameObjects

---

This package allows the user to define types or classes of GameObjects.
For example you can say that all GameObjects witch contains a `RigidBody` and
a `Collider` are `PhysicsObjects`. Such a type of GameObject is called an **aspect**.
You can then test if GameObjects have a defined aspect in order to guarantee that
it has required components.

```csharp

// Aspects are interfaces
// They must implement IGameObjectAspect
public interface IPhysicsObject : IGameObjectAspect {

    // Add component requirements using properties
    
    public RigidBody RigidBody { get; }
    
    public Collider Collider { get; }

}

// Attempt to get the aspect from some GameObject
var physicsObject = someGameObject.TryAspect<IPhysicsObject>();

// Will be null if it does not have the required components
if(phyicsObject != null) {

    // Do something with the components 
    physicsObject.RigidBody ...
    phsysicsObject.Collider ...
}

```

## Installation

### Compatibility

Designed with/for Unity 2021.3 and up. Should work on all build targets.

## Roadmap

- Find game-objects
- Find other aspects
- Support searching on parents
- Support searching in children
- Support searching in custom locations
- Array support
- Nullable support
- Live values
- Setter support
- Useful error messages
