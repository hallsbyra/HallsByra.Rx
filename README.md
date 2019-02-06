# HallsByra.Rx
You got to have them somewhere - your favorite extensions and helpers for RX. This is where I keep my little darlings.

For now, it's just these extensions to IObservable:

Extension|Description
---|---
`Multicast`|MultiCast with a factory that recreates/disposes its ISubject upon subscribe/unsubscribe.. 
`PublishBehavior`|Shorthand for Multicast with a BehaviorSubject factory.  
`PublishReplay`|Shorthand for Multicast with a ReplaySubject factory.
`SkipNull`|You guessed it. It skips null values.
`PairWithPrevious`|Pairs each element with its predecessor.

and this little helper:

Name|Description
---|---
`FileSystem.WhenFileChanges`|Observes a file for changes

## Multicast
This is basically just an ordinary Multicast but with a Subject factory instead of a fixed Subject. It means the the subscription to the source and the Subject are created each time the observer count is positive and disposed when there are no subscribers.

My typical use case goes like this:
```C#
var settings = 
    FileSystem.WhenFileChanges(@".\settings.json")
    .Select(File.ReadAllText)
    .PublishReplay(() => File.ReadAllText(@".\settings.json"));
```
The above code has the following properties:

* Returns settings from file when the first observer subscribes
* Returns cached settings to subsequence observers
* Disposes all resources when no one is observing 
* Only hits the file once on each update, regardless of the number of observers

Maybe there are other more elegant way of achieving this, but I haven't found out... 

## SkipNull
This one's only for the tough.

## PairWithPrevious
Combines each element with its predecessor. Cold and simple.