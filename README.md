# Unity GameObject Aspect

Aspect-oriented programming for Unity GameObjects

[![openupm](https://img.shields.io/npm/v/dev.comradevanti.gameobject-aspect?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/dev.comradevanti.gameobject-aspect/)

[Changelog](./CHANGELOG.md)

---

This package allows the user to define types or classes of GameObjects.
For example you can say that all GameObjects witch contains a `RigidBody` and
a `Collider` are `PhysicsObjects`. Such a type of GameObject is called an
**aspect**.

You can then test if GameObjects have a defined aspect in order to guarantee
that it has required components.

```csharp

// Aspects are interfaces
// They must implement IGameObjectAspect
public interface IPhysicsObject : IGameObjectAspect {

    // Add component requirements using read-only properties
    
    public RigidBody RigidBody { get; }
    
    public Collider Collider { get; }

    // Any other types of members, such as methods, are not allowed
}

// Attempt to get the aspect from some GameObject
var physicsObject = someGameObject.TryAspect<IPhysicsObject>();

// Will be null if game-object does not have the required components
if(phyicsObject != null) {

    // Do something with the components 
    physicsObject.RigidBody ...
    phsysicsObject.Collider ...
}

```

## Features

Currently properties of the following types are supported in aspects

- GameObject - Will always point to the host game-object
- Single Components - Searches for the component on the host game-object
    - This also works with interfaces such as searching for `IXRInteractable`

Attempt to get an aspect from a `GameObject` using the `TryAspect`extension
method.

```csharp
gameObject.TryAspect<IMyAspect>();

// You can also use TryAspect on components and other aspects

transform.TryAspect<IMyAspect>();
someAspect.TryAspect<IMyAspect>();
```

You can compare aspects using `Equals` which will be true if the stem from the
same game-object.

## Installation

Install via OpenUpm using `openupm add dev.comradevanti.gameobject-aspect`

### Compatibility

Designed with/for Unity 2021.3 and up. Should work on all build targets.

## How it works

We use `System.Emit` to generate an implementation class for your
aspect-interfaces at runtime. This has a slight performance penalty and so the
generated types are cached.

The `TryAspect` methods then attempt to populate all properties defined in the
aspect-interface by resolving the requested components on the given
host game-object.

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
- Allow to check if game-object has components without needing access to them (
  tag-components)