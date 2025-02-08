using System;
using PCRE.Internal;

#pragma warning disable CA1032

namespace PCRE;

/// <summary>
/// Represents errors returned by PCRE.
/// </summary>
public class PcreException : Exception
{
    /// <summary>
    /// The error code returned by PCRE.
    /// </summary>
    public PcreErrorCode ErrorCode { get; }

    /// <summary>
    /// Creates a <see cref="PcreException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    public PcreException(PcreErrorCode errorCode)
        : this(errorCode, Native.GetErrorMessage((int)errorCode), null)
    {
    }

    /// <summary>
    /// Creates a <see cref="PcreException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    public PcreException(PcreErrorCode errorCode, string message)
        : this(errorCode, message, null)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a <see cref="PcreException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreException(PcreErrorCode errorCode, string message, Exception? innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Represents an error that occured during pattern compilation.
/// </summary>
public class PcrePatternException : PcreException
{
    /// <summary>
    /// Creates a <see cref="PcrePatternException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    public PcrePatternException(PcreErrorCode errorCode, string message)
        : base(errorCode, message)
    {
    }
}

/// <summary>
/// Represents an error that occured during pattern matching.
/// </summary>
public class PcreMatchException : PcreException
{
    /// <summary>
    /// Creates a <see cref="PcreMatchException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    public PcreMatchException(PcreErrorCode errorCode)
        : base(errorCode)
    {
    }

    /// <summary>
    /// Creates a <see cref="PcreMatchException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreMatchException(PcreErrorCode errorCode, string message, Exception? innerException)
        : base(errorCode, message, innerException)
    {
    }
}

/// <summary>
/// Represents an error that occured during the execution of a callout.
/// </summary>
public class PcreCalloutException : PcreMatchException
{
    /// <summary>
    /// Creates a <see cref="PcreCalloutException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreCalloutException(string message, Exception? innerException)
        : base(PcreErrorCode.Callout, message, innerException)
    {
    }

    /// <summary>
    /// Creates a <see cref="PcreCalloutException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreCalloutException(PcreErrorCode errorCode, string message, Exception? innerException)
        : base(errorCode, message, innerException)
    {
    }
}

/// <summary>
/// Represents an error that occured during pattern substitution.
/// </summary>
public class PcreSubstituteException : PcreException
{
    /// <summary>
    /// Creates a <see cref="PcreSubstituteException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    public PcreSubstituteException(PcreErrorCode errorCode)
        : base(errorCode)
    {
    }

    /// <summary>
    /// Creates a <see cref="PcreSubstituteException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreSubstituteException(PcreErrorCode errorCode, string message, Exception? innerException)
        : base(errorCode, message, innerException)
    {
    }
}
