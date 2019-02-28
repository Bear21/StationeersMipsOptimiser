# StationeersMipsOptimiser
Removes un-needed lines from IC10 code for Stationeers


# Example usage
Optimising my following hardsuit control program named `suitcontrol.txt` is archieved simply by passing a text file containing the code in as first parameter
### Optimised
```
$ dotnet StationeersMipsOptimiser.dll suitcontrol.txt
yield
bdns d0 0
l r0 d0 Open
s d0 Lock 1
yield
bdns d0 0
l r2 db TemperatureExternal
blt r2 278 20
bgt r2 311 20
l r2 db PressureExternal
blt r2 35 20
bgt r2 125 20
move r3 1
beq r3 r0 4
seq r0 r3 1
seq r1 r3 0
s d0 Open r0
s db Filtration r1
s db On r1
j 4
move r3 0
j 13
```
### Human Readable
```
$ cat suitcontrol.txt
alias helmet d0
alias jetpack d1
alias suit db
alias systemState r0
alias systemStateInversed r1
alias temperature r2
alias pressure r2 # Union with temperature.
alias newState r3

DetectSystemState:
    yield
    bdns helmet DetectSystemState
    l systemState helmet Open
    s helmet Lock 1

    start:
        yield
        # Check for hardware
        bdns helmet DetectSystemState
        # We don't need a jetpack for this program.
        #bdns jetpack start

        TempTest:
            l temperature suit TemperatureExternal
            blt temperature 278 FailedTest
            bgt temperature 311 FailedTest
        # Success TempTest

        PressureTest:
            l pressure suit PressureExternal
            blt pressure 35 FailedTest
            bgt pressure 125 FailedTest
        # Success PressureTest

        ToxinsTest:
        # idk?

        move newState 1 # Show success

        ApplyNewState:
            beq newState systemState start
            seq systemState newState 1
            seq systemStateInversed newState 0
            s helmet Open systemState
            s suit Filtration systemStateInversed
            s suit On systemStateInversed
        #
        j start
    #

    FailedTest:
        move newState 0
        j ApplyNewState
    #

```
