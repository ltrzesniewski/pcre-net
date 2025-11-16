using System;
using PCRE.Internal;

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
    /// Creates a new <see cref="PcreException"/>.
    /// </summary>
    public PcreException()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PcreException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    public PcreException(PcreErrorCode errorCode)
        : base(default(Native16Bit).GetErrorMessage((int)errorCode))
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a new <see cref="PcreException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    public PcreException(PcreErrorCode errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a new <see cref="PcreException"/>.
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
    /// Creates a new <see cref="PcrePatternException"/>.
    /// </summary>
    public PcrePatternException()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcrePatternException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PcrePatternException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcrePatternException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcrePatternException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcrePatternException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    public PcrePatternException(PcreErrorCode errorCode, string message)
        : base(errorCode, message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcrePatternException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcrePatternException(PcreErrorCode errorCode, string message, Exception? innerException)
        : base(errorCode, message, innerException)
    {
    }
}

/// <summary>
/// Represents an error that occured during pattern matching.
/// </summary>
public class PcreMatchException : PcreException
{
    /// <summary>
    /// Creates a new <see cref="PcreMatchException"/>.
    /// </summary>
    public PcreMatchException()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreMatchException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PcreMatchException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreMatchException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreMatchException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreMatchException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    public PcreMatchException(PcreErrorCode errorCode)
        : base(errorCode)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreMatchException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    public PcreMatchException(PcreErrorCode errorCode, string message)
        : base(errorCode, message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreMatchException"/>.
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
    /// Creates a new <see cref="PcreCalloutException"/>.
    /// </summary>
    public PcreCalloutException()
        : base(PcreErrorCode.Callout)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreCalloutException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PcreCalloutException(string message)
        : base(PcreErrorCode.Callout, message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreCalloutException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreCalloutException(string message, Exception? innerException)
        : base(PcreErrorCode.Callout, message, innerException)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreCalloutException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    public PcreCalloutException(PcreErrorCode errorCode)
        : base(errorCode)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreCalloutException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    public PcreCalloutException(PcreErrorCode errorCode, string message)
        : base(errorCode, message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreCalloutException"/>.
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
    /// Creates a new <see cref="PcreSubstituteException"/>.
    /// </summary>
    public PcreSubstituteException()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreSubstituteException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PcreSubstituteException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreSubstituteException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreSubstituteException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreSubstituteException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    public PcreSubstituteException(PcreErrorCode errorCode)
        : base(errorCode)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreSubstituteException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    public PcreSubstituteException(PcreErrorCode errorCode, string message)
        : base(errorCode, message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PcreSubstituteException"/>.
    /// </summary>
    /// <param name="errorCode">The associated error code.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PcreSubstituteException(PcreErrorCode errorCode, string message, Exception? innerException)
        : base(errorCode, message, innerException)
    {
    }
}
