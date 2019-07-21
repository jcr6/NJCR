# NJCR

NJCR is a CLI tool for JCR6 written in .NET

# Why?

Although the current tools written in Go do their job, there are some serious issues:

- Go has some syntax issues that I don't wish to solve. Let's say, some parts of Go were never well thought-out, and that makes coding sometimes needlessly complicated
- Upper start = Public and lower start = private was a bad idea!
- Go's strictness on unused stuff makes debugging HELL! Forces me to disable stuff I've set up for later usage, only to be forgotten later leading to new parse errors or bugs.
- Go is too slow, even on an extremely fast computer.
- I wasn't happy about the current set up of the CLI tools anyway, so a rewrite was about to be in order anyway.

# Does that mean the Go tools will be discontinued

- On the short run.... no
- On the longer run, that will depend. I delayed this all the time for one reason. I could not properly set up all compression drivers well in C# that I had at my disposal in Go. When that issue's been solved, then very likely the Go version will be discontinued.

# Hey! Does that mean I can no longer run JCR6 on Linux and Mac?

Of course you can. Make sure Mono is installed!

# Does the CLI syntax change much?

Depends per feature. Some parts will require a complete redo... Others will suffice the way they were and thus be handled as much accordingly.

# License

These tools will be licenced under the GPL 3, the drivers for JCR6 are not part of NJCR... They are only merged in as a dependency, and they will therefore have their own licenses.
