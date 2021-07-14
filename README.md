# Megaprimes
 Small C# library for calculating megaprimes of N. 
 
 The prime number generator algorithm utilized by MegaprimesLib is the Sieve of Eratosthenes algorithm (https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes) with additional heuristics and parallelism in order to achive the desired functionality in a performant manner.
 
 This application was built using Visual Studio 2019, and targets .Net 5.0.
 
# What is a megaprime 
A prime number is an integer greater than 1 that has no positive divisors other than 1 and itself.

We call a number megaprime if it is prime and all its individual digits are prime. For example, 53 is
megaprime because it is prime and all its digits (5 and 3) are prime; however, 35 is not megaprime
because it is not prime (it's divisible by 5 and 7), and 13 is not megaprime because it has a non-prime
digit (1 is not prime).

# Requirements 

* Visual Studio 2019 / Dotnet Cli Tool
* Net 5.0

# Structure 

## MegaprimesLib

__MegaprimesLib/MegaprimeGenerator.cs__

* Core megaprime logic containing prime number generation, and megaprime selection / output.

__MegaprimesLib/CPUInfo.cs__
* CPU cache information provider *Only available in Windows OS environment.

## MegaprimesLib.Tests

### ___MegaprimesLib.Tests/GeneratorTestCase/UpTo___
* Test cases for ___MegaprimesLib___ Generator class ```UpTo(UInt32)``` Method.



# Example Use

```csharp
using System;
using MegaprimesLib;

namespace MegaprimesCLIExample
{
    class Program
    {
        static void Main (string[] args)
        {
            var i = 0U;
            
            foreach (var MPrime in MegaprimeGenerator.UpTo(75327))
            {
                Console.WriteLine($"Megaprime {i} : {MPrime}");

                i++;
            }
        }
    }
}

```

# Approximate Benchmarks
#### ___```MegaprimeGenerator.UpTo(UInt32)```___ :

|      Specified MaxValue       |  Megaprime Search Space  | Time to Complete (ms) |
|          -----------          |        -----------       |       -----------     |
| 10                            | 0 .. 10                  | < 1                   |
| 37                            | 0 .. 37                  | < 1                   |
| 1121209984                    | 0 .. 1121209984          | 8425                  |
| 3173757311                    | 0 .. 3173757311          | 25 225                |
| 4294967295                    | 0 .. 4294967295          | 34 500                |
