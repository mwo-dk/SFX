module SFX.Random

open SFX.Random.CSharp

/// Generates a random bool
let getRandomBool = RandomHelpers.GetRandomBool
/// Generates a random signed byte
let getRandomSignedByte = RandomHelpers.GetRandomSignedByte
/// Generates a random char
let getRandomChar = RandomHelpers.GetRandomChar
/// Generates a random short
let getRandomShort = RandomHelpers.GetRandomShort
/// Generates a random int
let getRandomInt = RandomHelpers.GetRandomInt
/// Generates a random long
let getRandomLong = RandomHelpers.GetRandomLong
/// Generates a random byte
let getRandomByte = RandomHelpers.GetRandomByte
/// Generates a random unsigned short
let getRandomUnsignedShort = RandomHelpers.GetRandomUnsignedShort
/// Generates a random unsigned int
let getRandomUnsignedInt = RandomHelpers.GetRandomUnsignedInt
/// Generates a random unsigned long
let getRandomUnsignedLong = RandomHelpers.GetRandomUnsignedLong
/// Generates a random float32
let getRandomFloat = RandomHelpers.GetRandomFloat
/// Generates a random float
let getRandomDouble = RandomHelpers.GetRandomDouble
/// Generates a random decimal
let getRandomDecimal = RandomHelpers.GetRandomDecimal
/// Generates a random TimeSpan
let getRandomTimeSpan = RandomHelpers.GetRandomTimeSpan
/// Generates a random DateTimeOffset
let getRandomDateTimeOffset = RandomHelpers.GetRandomDateTimeOffset

/// Generates a random signed byte in a given range
let getRandomSignedByteInRange a b = RandomHelpers.GetRandomSignedByteInRange(a, b)
/// Generates a random char in a given range
let getRandomCharInRange a b = RandomHelpers.GetRandomCharInRange(a, b)
/// Generates a random short in a given range
let getRandomShortInRange a b = RandomHelpers.GetRandomShortInRange(a, b)
/// Generates a random int in a given range
let getRandomIntInRange a b = RandomHelpers.GetRandomIntInRange(a, b)
/// Generates a random long in a given range
let getRandomLongInRange a b = RandomHelpers.GetRandomLongInRange(a, b)
/// Generates a random byte in a given range
let getRandomByteInRange a b = RandomHelpers.GetRandomByteInRange(a, b)
/// Generates a random unsigned short in a given range
let getRandomUnsignedShortInRange a b = RandomHelpers.GetRandomUnsignedShortInRange(a, b)
/// Generates a random unsigned int in a given range
let getRandomUnsignedIntInRange a b = RandomHelpers.GetRandomUnsignedIntInRange(a, b)
/// Generates a random unsigned long in a given range
let getRandomUnsignedLongInRange a b = RandomHelpers.GetRandomUnsignedLongInRange(a, b)
/// Generates a random float32 in a given range
let getRandomFloatInRange a b = RandomHelpers.GetRandomFloatInRange(a, b)
/// Generates a random float in a given range
let getRandomDoubleInRange a b = RandomHelpers.GetRandomDoubleInRange(a, b)
/// Generates a random decimal in a given range
let getRandomDecimalInRange a b = RandomHelpers.GetRandomDecimalInRange(a, b)
/// Generates a random TimeSpan in a given range
let getRandomTimeSpanInRange a b = RandomHelpers.GetRandomTimeSpanInRange(a, b)
/// Generates a random DateTimeOffset in a given range
let getRandomDateTimeOffsetInRange a b = RandomHelpers.GetRandomDateTimeOffsetInRange(a, b)