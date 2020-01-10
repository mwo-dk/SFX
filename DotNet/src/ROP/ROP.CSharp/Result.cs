using System;

namespace SFX.CSROP
{
    /// <summary>
    /// Represents the result of an operation/invokation. It is not a full two track
    /// input representation, since it:
    /// 1) Insists that the error type is <see cref="Exception"/>
    /// 2) Enables default "fallback" to exception typed flow via an implicit cast
    /// operator, that throws the provided exception if one has been provided
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of a success full outcome</typeparam>
    public struct Result<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The optional value</param>
        /// <param name="error">The optional <see cref="Exception"/></param>
        internal Result(T value, Exception error) =>
            (Value, Error) = (value, error);

        /// <summary>
        /// The result of the operation if successful
        /// </summary>
        public T Value { get; }
        /// <summary>
        /// The error of the operation if not that successfull
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// Deconstructor
        /// </summary>
        /// <param name="success">Flag telling whether the operation was successfull</param>
        /// <param name="value">The result - if successfull</param>
        /// <param name="error">The <see cref="Exception"/> if present</param>
        public void Deconstruct(out bool success, out T value, out Exception error) =>
            (success, value, error) = (Error is null, Value, Error);

        /// <summary>
        /// Implicit cast to the value if the operation was successfull
        /// </summary>
        /// <param name="x">The <see cref="Result{T}"/> to cast</param>
        public static implicit operator T(Result<T> x)
        {
            if (!(x.Error is null))
                throw x.Error;
            else return x.Value;
        }

        /// <summary>
        /// Implicit cast to briding type <see cref="ROP.Bridge.Result"/> in
        /// order to facilitate bridging between the F# sum-type and this simpler type
        /// </summary>
        /// <param name="x">The <see cref="Result{T}"/> to cast</param>
        public static implicit operator ROP.Bridge.Result<T>(Result<T> x) =>
            new ROP.Bridge.Result<T>(x.Value, x.Error);
    }
}
