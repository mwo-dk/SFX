# SFX.Gower
Implementation of Gower similarity coefficient as described in [the paper](http://members.cbio.mines-paristech.fr/~jvert/svn/bibli/local/Gower1971general.pdf). The repository contains two packages:

* [SFX.Gower](https://www.nuget.org/packages/SFX.Gower/) for F#, and
* [SFX.Gower.CSharp](https://www.nuget.org/packages/SFX.Gower.CSharp/) for C#

The packages are not dependent of one another.

The idea is, that you may provide some type of sample ```T```, that has various characters (fields/properties), that you might want to contribute to the Gower similarity score. You then build up a Gower configuration object, where you append the scores (builder), and then finally get a function, that will be able to compute the Gower similarity coefficient between to samples.

Weighting is not implemented yet, but will come some day.

## Usage C#

``` csharp
public enum SomeQuality {
    ....
}
public class Sample {
    public bool AsymmmetricDichotomousProperty {get;set;}
    public bool SymmmetricDichotomousProperty {get;set;}
    public SomeQuality Quality {get;set;}
    public double Quantity1 {get;set}
    public double Quantity2 {get;set;} 
}

...

var conf = new GowerConfiguration<Sample>()
    .WithAsymmetricDichotomousCharacter(x => x.AsymmmetricDichotomousProperty)
    .WithSymmetricDichotomousCharacter(x => x.SymmmetricDichotomousProperty)
    .WithQualitativeCharacter(x => x.Quality)
    .WithQuantitativeCharacter(x => x.Quantity1)
    .WithQuantitativeCharacter(x => x.Quantity2);

var gower = conf.FromConfig(); // Has type Func<Sample, Sample, IEnumerable<double>, (double, bool)>
``` 

The Gower function generated, is supposed to be utilized the following way:

* If any quantititive characters are present, the range for each has to be determined (in order of appended to the configuration).
* Invoke the method in the manner: (x, y, ranges). The result is a tuple, where the last item determines whether the result is valid, and if so, the result

## Usage F#

``` fsharp
type SomeQuality =
| ...

type Sample = {
    AsymmmetricDichotomousProperty: bool
    SymmmetricDichotomousProperty: bool
    Quality: SomeQuality
    Quantity1: float
    Quantity2: float
}

...

let conf = 
    createGowerConfiguration() |>
    withAsymmetricDichotomousCharacter (fun x -> x.AsymmmetricDichotomousProperty) |>
    withSymmetricDichotomousCharacter (fun x -> x.SymmmetricDichotomousProperty) |>
    withQualitativeCharacter (fun x -> x.Quality) |>
    withQuantitativeCharacter (fun x -> x.Quantity1) |>
    withQuantitativeCharacter (fun x -> x.Quantity2) 

let gower = conf |> fromConfig // Has signature: 'a -> 'a -> float list -> float*bool
```