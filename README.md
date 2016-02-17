# HallsByra.Rx
You got to have them somewhere - your favorite extensions and helpers for RX. This is where I keep my little darlings.

For now, it's just these extensions to IObservable:

Extension|Description
-|-
`Behave`|Applies the functionality of a BehaviorSubject to an IObservable  
`CacheLast`|"Weakly" caches the last element.
`MulticastWeak`|MultiCast that recreates/disposes its ISubject upon subscribe/unsubscribe.. 
`SkipNull`|You guessed it. It skips null values.
`PairWithPrevious`|Pairs each element with its predecessor.

and this little helper:

Name|Description
-|-
`FileSystem.WhenFileChanges`|Observes a file for changes

##Behave
Provides BehaviorSubject-like functionality for an IObservable. 
Maintains a single subscription to the source and pushes all elements to observers through a BehaviorSubject.
The subscription to the source and the BehaviorSubject are  created each time the observer count is positive and
disposed when there are no subscribers.

My typical use case goes like this:
```C#
var settings = 
    FileSystem.WhenFileChanges(@".\settings.json")
    .Select(File.ReadAllText)
    .Behave(() => File.ReadAllText(@".\settings.json"));
```
The above code has the following properties:

* Returns settings from file when the first observer subscribes
* Returns cached settings to subsequence observers
* Disposes all resources when no one is observing 
* Only hits the file once on each update, regardless of the number of observers

Maybe there are other more elegant way of achieving this, but I haven't found out... 


## CacheLast
Makes the last seen element immediately available to new observers.
Much like `Replay(1)` but with the subtle difference that `CacheLast` only keeps the last element cached as long as it has any observers.
When the last observer is gone, so is the cache.

The example from `Behave` could have been written using `CacheLast` like this:

```C#
var settings =
    Observable.Defer(() => Observable.Return(@".\settings.json"))
    .Concat(FileSystem.WhenFileChanges(@".\settings.json"))
    .Select(File.ReadAllText)
    .CacheLast();
```

## MulticastWeak
More or less like the standard `Multicast` but recreates the `ISubject` each time the subscription count is positive and
disposes the `ISubject` and the source subscription when the subscription count reaches zero.

`Behave` and `CacheLast` are simple wrappers around MulticastWeak like so (in principle):

```C#
CacheLast = source.MulticastWeak(() => new ReplaySubject<T>(1));
Behave = source.MulticastWeak(() => new BehaviorSubject<T>());
```

## SkipNull
This one's only for the tough.

## PairWithPrevious
Combines each element with its predecessor. Cold and simple.