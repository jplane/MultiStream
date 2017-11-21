# MultiStream

This is a .NET Standard library for aggregating read-only streams, so that classes like StreamReader can treat multiple input streams as a single contiguous block of bytes.

Usage:

```C#

var ms1 = GetStream("hello world");

var ms2 = GetStream("goodbye world");

var s = new Lib.MultiStream(ms1, ms2);

var reader = new StreamReader(s);

var output = reader.ReadToEnd();

Assert.AreEqual("hello worldgoodbye world", output);

```

The repo includes a .NET Core unit test project and a .NET Framework console app that both demonstrate basic usage.
