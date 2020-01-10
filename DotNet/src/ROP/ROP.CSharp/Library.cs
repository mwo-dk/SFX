using System;
using static SFX.ROP;

namespace SFX.CSROP
{
    /// <summary>
    /// Wanna-be C# ROP module
    /// </summary>
    public static class Library
    {
        /// <summary>
        /// Returns an <see cref="Result{T}"/> denoting success
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the result</typeparam>
        /// <param name="value">The successfull result</param>
        /// <returns>An <see cref="Result{T}"/> denoting success</returns>
        public static Result<T> Succeed<T>(T value) =>
            new Result<T>(value, default);

        /// <summary>
        /// Returns an <see cref="Result{T}"/> denoting failure
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the failed result</typeparam>
        /// <param name="error">The error of the operation</param>
        /// <returns>An <see cref="Result{T}"/> denoting success</returns>
        public static Result<T> Fail<T>(Exception error) =>
            new Result<T>(default, error);


        #region Either
        /// <summary>
        /// Invokes the appropriate handler depending on the state of the provided <paramref name="twoTrackInput"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the successfull outcome of the original operation</typeparam>
        /// <param name="twoTrackInput">The input representing either a successfull or a failed previous operation</param>
        /// <param name="successHandler">The handler to be invoked in case of success</param>
        /// <param name="errorHandler">The handler to be invoked in case of failure (previously)</param>
        /// <returns></returns>
        public static void Either<T>(this Result<T> twoTrackInput,
            Action<T> successHandler, Action<Exception> errorHandler)
        {
            var (ok, result, error) = twoTrackInput;
            if (ok) successHandler(result);
            else errorHandler(error);
        }

        /// <summary>
        /// Invokes the appropriate handler depending on the state of the provided <paramref name="twoTrackInput"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the successfull outcome of the original operation</typeparam>
        /// <param name="twoTrackInput">The input representing either a successfull or a failed previous operation</param>
        /// <param name="successHandler">The handler to be invoked in case of success</param>
        /// <param name="errorHandler">The handler to be invoked in case of failure (previously)</param>
        /// <returns></returns>
        public static T Either<T>(this Result<Unit> twoTrackInput,
            Func<T> successHandler, Func<Exception, T> errorHandler)
        {
            var (ok, _, error) = twoTrackInput;
            if (ok) return successHandler();
            else return errorHandler(error);
        }

        /// <summary>
        /// Invokes the appropriate handler depending on the state of the provided <paramref name="twoTrackInput"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the successfull outcome of the original operation</typeparam>
        /// <typeparam name="U">The <see cref="Type"/> of the operation</typeparam>
        /// <param name="twoTrackInput">The input representing either a successfull or a failed previous operation</param>
        /// <param name="successHandler">The handler to be invoked in case of success</param>
        /// <param name="errorHandler">The handler to be invoked in case of failure (previously)</param>
        /// <returns></returns>
        public static U Either<T, U>(this Result<T> twoTrackInput,
            Func<T, U> successHandler, Func<Exception, U> errorHandler)
        {
            var (ok, result, error) = twoTrackInput;
            if (ok) return successHandler(result);
            else return errorHandler(error);
        }
        #endregion

        #region Bind
        public static Action<Result<T>> Bind<T>(Action<T> f) =>
            new Action<Result<T>>(x => Either(x, f, error => Fail<Unit>(error)));

        public static Func<Result<T>> Bind<T>(Func<Result<T>> f) =>
            new Func<Result<T>>(() => Either(Succeed<Unit>(Unit.Value), f, error => Fail<T>(error)));

        public static Func<Result<T>, Result<U>> Bind<T, U>(Func<T, Result<U>> f) =>
            new Func<Result<T>, Result<U>>(x => Either(x, f, error => Fail<U>(error)));
        #endregion

        #region Switch
        public static Func<T, Result<Unit>> Switch<T, U>(Action<T> f) =>
            new Func<T, Result<Unit>>(x =>
            {
                f(x);
                return Succeed(Unit.Value);
            });

        public static Func<Result<T>> Switch<T>(Func<T> f) =>
            new Func<Result<T>>(() => Succeed(f()));

        public static Func<T, Result<U>> Switch<T, U>(Func<T, U> f) =>
            new Func<T, Result<U>>(x => Succeed(f(x)));
        #endregion

        #region TryCatch
        public static Action<T> TryCatch<T, U>(Action<T> f,
            Func<Exception, U> errorHAndler) where U : Exception =>
            new Action<T>(x =>
            {
                try
                {
                    f(x);
                }
                catch (Exception error)
                {
                    errorHAndler(error);
                }
            });

        public static Func<Result<T>> TryCatch<T, U>(Func<T> f,
            Func<Exception, U> errorHAndler) where U : Exception =>
            new Func<Result<T>>(() =>
            {
                try
                {
                    return Succeed(f());
                }
                catch (Exception error)
                {
                    return Fail<T>(errorHAndler(error));
                }
            });

        public static Func<T, Result<U>> TryCatch<T, U>(Func<T, U> f,
            Func<Exception, U> errorHAndler) where U : Exception =>
            new Func<T, Result<U>>(x =>
            {
                try
                {
                    return Succeed(f(x));
                }
                catch (Exception error)
                {
                    return Fail<U>(errorHAndler(error));
                }
            });
        #endregion

        public static Result<T> ToResult<T>(this Result<T, Exception> result) =>
            result switch
            {
                Result<T, Exception>.Success x => Succeed(x.Item),
                Result<T, Exception>.Failure err => Fail<T>(err.Item),
                _ => Fail<T>(new Exception()) // Should never happen
            };
    }
}
