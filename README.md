# Todo
Hey! Welcome to the TODO github repository. So, if you look around you will notice something... TODO has more than a few TODO's itself. Whoops! Well, it will be ready sooner or later. In earlier pushes of this repository (some of which have been force-pushed into Oblivion for *good reason*), this README didn't have much, other than just saying it would be done soon^(tm)... Well, soon is actually looking like a reality at this point, so this document will outline the philosophy / use of TODO.

# About
So, in case you're out of the loop, developers like TODO comments. If they can't finish what they're working on at any given moment, they like to just slap a "//TODO: Do this" down into the code and keep on working. In some of my projects, a `grep -R "todo" | wc -l` would give back 45 / 50 lines! So, I set out to create a tool to manage all of these TODOS.

But where should the extensibility end? Should it listen to just TODOs in forward slashes? Should it allow require a colon? Any advanced features?

So that is where TODO's extensibility comes into play.

# Extensibility via Globs
Globs sound nasty. But they're super handy. Globs is a super light "language" I use for metadata (lighter, yet less featureful than JSON). Because, let's face it... if you're writing json TODO comments, you might as well write a manifesto of what you intend to do; or better yet, implement it.

Globs look like this:

`here are words and{here is a glob}`. and{here is a glob} is considered one "word" by globs. Globs reads items split up by commas inside of the braces as different elements. `"my favorite ice cream flavor is: favorite{chocolate,coffee}"`. So, how about that? Someone can't choose their favorite flavor. favorite, the globs key, is now bound to chocolate and coffee.

This is set up with a ordered-pair esque structure, which represents this as {(favorite, chocolate), (favorite, coffee)}... Why does any of this nitty gritty implementation stuff matter? Because here's why!

TODO, by default, defines what to do with 2 glob identifiers: dependencies and name. Eventually, I wanna have TODO preprocess +{ into name{ and !{ into dependency to make this easier. But it lets you do stuff like this:

    TODO name{job1}: This is the first job
    TODO name{job2} dependencies{job1}: This is the second job
	TODO name{job3} dependencies{job2}: This is the third job

So, when you generate the todo **contract** for the named Todo "job3", it will give you job1 and job2 to do first since they're dependencies.

TODO automatically checks for circular dependencies. This is the one part of the project that currently works.

# Future Plans
TODO works... Ish. The base code works, but the program is set up as a test. So if you wanna use Task and TaskTable then go ahead. But my plans include:

* Making Parsing more forgiving  
* Integrating with HumDrum.Operations.DirectorySearch  
* Implementing the command line arguments

In other words, the program part of the program. Fun!

# License
Everything in this repository is BSD 3 clause. Do with it as you will. 
