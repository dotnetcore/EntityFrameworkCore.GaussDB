// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Provides extension methods for multiranges supporting PostgreSQL translation.
/// </summary>
public static class GaussDBMultirangeDbFunctionsExtensions
{
    #region Contains

    /// <summary>
    ///     Determines whether a multirange contains a specified value.
    /// </summary>
    /// <param name="multirange">The multirange in which to locate the value.</param>
    /// <param name="value">The value to locate in the range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified value; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Contains{T}(GaussDBRange{T}[], T)" /> is only intended for use via SQL translation as part of an EF Core LINQ query.
    /// </exception>
    public static bool Contains<T>(this GaussDBRange<T>[] multirange, T value)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Contains)));

    /// <summary>
    ///     Determines whether a multirange contains a specified value.
    /// </summary>
    /// <param name="multirange">The multirange in which to locate the value.</param>
    /// <param name="value">The value to locate in the range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified value; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Contains{T}(List{GaussDBRange{T}}, T)" /> is only intended for use via SQL translation as part of an EF Core LINQ query.
    /// </exception>
    public static bool Contains<T>(this List<GaussDBRange<T>> multirange, T value)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Contains)));

    /// <summary>
    ///     Determines whether a multirange contains a specified multirange.
    /// </summary>
    /// <param name="multirange1">The multirange in which to locate the specified multirange.</param>
    /// <param name="multirange2">The specified multirange to locate in the multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Contains{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool Contains<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Contains)));

    /// <summary>
    ///     Determines whether a multirange contains a specified multirange.
    /// </summary>
    /// <param name="multirange1">The multirange in which to locate the specified multirange.</param>
    /// <param name="multirange2">The specified multirange to locate in the multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Contains{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static bool Contains<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Contains)));

    /// <summary>
    ///     Determines whether a multirange contains a specified range.
    /// </summary>
    /// <param name="multirange1">The multirange in which to locate the specified range.</param>
    /// <param name="multirange2">The specified range to locate in the multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Contains{T}(GaussDBRange{T}[], GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static bool Contains<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Contains)));

    /// <summary>
    ///     Determines whether a multirange contains a specified range.
    /// </summary>
    /// <param name="multirange1">The multirange in which to locate the specified range.</param>
    /// <param name="multirange2">The specified range to locate in the multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Contains{T}(List{GaussDBRange{T}}, GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool Contains<T>(this List<GaussDBRange<T>> multirange1, GaussDBRange<T> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Contains)));

    #endregion Contains

    #region ContainedBy

    /// <summary>
    ///     Determines whether a multirange is contained by a specified multirange.
    /// </summary>
    /// <param name="multirange1">The specified multirange to locate in the multirange.</param>
    /// <param name="multirange2">The multirange in which to locate the specified multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="ContainedBy{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool ContainedBy<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ContainedBy)));

    /// <summary>
    ///     Determines whether a multirange is contained by a specified multirange.
    /// </summary>
    /// <param name="multirange1">The specified multirange to locate in the multirange.</param>
    /// <param name="multirange2">The multirange in which to locate the specified multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="ContainedBy{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool ContainedBy<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ContainedBy)));

    /// <summary>
    ///     Determines whether a range is contained by a specified multirange.
    /// </summary>
    /// <param name="range">The specified range to locate in the multirange.</param>
    /// <param name="multirange">The multirange in which to locate the specified range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="ContainedBy{T}(GaussDBRange{T}, GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool ContainedBy<T>(this GaussDBRange<T> range, GaussDBRange<T>[] multirange)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ContainedBy)));

    /// <summary>
    ///     Determines whether a range is contained by a specified multirange.
    /// </summary>
    /// <param name="range">The specified range to locate in the multirange.</param>
    /// <param name="multirange">The multirange in which to locate the specified range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange contains the specified range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="ContainedBy{T}(GaussDBRange{T}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static bool ContainedBy<T>(this GaussDBRange<T> range, List<GaussDBRange<T>> multirange)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ContainedBy)));

    #endregion ContainedBy

    #region Overlaps

    /// <summary>
    ///     Determines whether a multirange overlaps another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multiranges overlap (share points in common); otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Overlaps{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool Overlaps<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Overlaps)));

    /// <summary>
    ///     Determines whether a multirange overlaps another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multiranges overlap (share points in common); otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Overlaps{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool Overlaps<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Overlaps)));

    /// <summary>
    ///     Determines whether a multirange overlaps another range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange and range overlap (share points in common); otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Overlaps{T}(GaussDBRange{T}[], GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static bool Overlaps<T>(this GaussDBRange<T>[] multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Overlaps)));

    /// <summary>
    ///     Determines whether a multirange overlaps another range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange and range overlap (share points in common); otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Overlaps{T}(List{GaussDBRange{T}}, GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool Overlaps<T>(this List<GaussDBRange<T>> multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Overlaps)));

    #endregion Overlaps

    #region IsStrictlyLeftOf

    /// <summary>
    ///     Determines whether a multirange is strictly to the left of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange is strictly to the left of the second multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyLeftOf{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static bool IsStrictlyLeftOf<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyLeftOf)));

    /// <summary>
    ///     Determines whether a multirange is strictly to the left of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange is strictly to the left of the second multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyLeftOf{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part
    ///     of an EF Core LINQ query.
    /// </exception>
    public static bool IsStrictlyLeftOf<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyLeftOf)));

    /// <summary>
    ///     Determines whether a multirange is strictly to the left of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange is strictly to the left of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyLeftOf{T}(GaussDBRange{T}[], GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static bool IsStrictlyLeftOf<T>(this GaussDBRange<T>[] multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyLeftOf)));

    /// <summary>
    ///     Determines whether a multirange is strictly to the left of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange is strictly to the left of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyLeftOf{T}(List{GaussDBRange{T}}, GaussDBRange{T})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool IsStrictlyLeftOf<T>(this List<GaussDBRange<T>> multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyLeftOf)));

    #endregion IsStrictlyLeftOf

    #region IsStrictlyRightOf

    /// <summary>
    ///     Determines whether a multirange is strictly to the right of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange is strictly to the right of the second multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyRightOf{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool IsStrictlyRightOf<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyRightOf)));

    /// <summary>
    ///     Determines whether a multirange is strictly to the right of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange is strictly to the right of the second multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyRightOf{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part
    ///     of an EF Core LINQ query.
    /// </exception>
    public static bool IsStrictlyRightOf<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyRightOf)));

    /// <summary>
    ///     Determines whether a multirange is strictly to the right of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange is strictly to the right of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyRightOf{T}(GaussDBRange{T}[], GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static bool IsStrictlyRightOf<T>(this GaussDBRange<T>[] multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyRightOf)));

    /// <summary>
    ///     Determines whether a multirange is strictly to the right of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange is strictly to the right of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsStrictlyRightOf{T}(List{GaussDBRange{T}}, GaussDBRange{T})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool IsStrictlyRightOf<T>(this List<GaussDBRange<T>> multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsStrictlyRightOf)));

    #endregion IsStrictlyRightOf

    #region DoesNotExtendLeftOf

    /// <summary>
    ///     Determines whether a multirange does not extend to the left of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange does not extend to the left of the multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendLeftOf{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendLeftOf<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendLeftOf)));

    /// <summary>
    ///     Determines whether a multirange does not extend to the left of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange does not extend to the left of the multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendLeftOf{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as
    ///     part of an EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendLeftOf<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendLeftOf)));

    /// <summary>
    ///     Determines whether a multirange does not extend to the left of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange does not extend to the left of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendLeftOf{T}(GaussDBRange{T}[], GaussDBRange{T})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendLeftOf<T>(this GaussDBRange<T>[] multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendLeftOf)));

    /// <summary>
    ///     Determines whether a multirange does not extend to the left of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange does not extend to the left of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendLeftOf{T}(List{GaussDBRange{T}}, GaussDBRange{T})" /> is only intended for use via SQL translation as part of
    ///     an EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendLeftOf<T>(this List<GaussDBRange<T>> multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendLeftOf)));

    #endregion DoesNotExtendLeftOf

    #region DoesNotExtendRightOf

    /// <summary>
    ///     Determines whether a multirange does not extend to the right of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange does not extend to the right of the multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendRightOf{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of
    ///     an EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendRightOf<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendRightOf)));

    /// <summary>
    ///     Determines whether a multirange does not extend to the right of another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the first multirange does not extend to the right of the multirange; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendRightOf{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as
    ///     part of an EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendRightOf<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendRightOf)));

    /// <summary>
    ///     Determines whether a multirange does not extend to the right of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange does not extend to the right of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendRightOf{T}(GaussDBRange{T}[], GaussDBRange{T})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendRightOf<T>(this GaussDBRange<T>[] multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendRightOf)));

    /// <summary>
    ///     Determines whether a multirange does not extend to the right of a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange does not extend to the right of the range; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="DoesNotExtendRightOf{T}(List{GaussDBRange{T}}, GaussDBRange{T})" /> is only intended for use via SQL translation as part of
    ///     an EF Core LINQ query.
    /// </exception>
    public static bool DoesNotExtendRightOf<T>(this List<GaussDBRange<T>> multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DoesNotExtendRightOf)));

    #endregion DoesNotExtendRightOf

    #region IsAdjacentTo

    /// <summary>
    ///     Determines whether a multirange is adjacent to another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multiranges are adjacent; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsAdjacentTo{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool IsAdjacentTo<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsAdjacentTo)));

    /// <summary>
    ///     Determines whether a multirange is adjacent to another multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multiranges are adjacent; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsAdjacentTo{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of
    ///     an EF Core LINQ query.
    /// </exception>
    public static bool IsAdjacentTo<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsAdjacentTo)));

    /// <summary>
    ///     Determines whether a multirange is adjacent to a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange and range are adjacent; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsAdjacentTo{T}(GaussDBRange{T}[], GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static bool IsAdjacentTo<T>(this GaussDBRange<T>[] multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsAdjacentTo)));

    /// <summary>
    ///     Determines whether a multirange is adjacent to a range.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    ///     <value>true</value>
    ///     if the multirange and range are adjacent; otherwise,
    ///     <value>false</value>
    ///     .
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="IsAdjacentTo{T}(List{GaussDBRange{T}}, GaussDBRange{T})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static bool IsAdjacentTo<T>(this List<GaussDBRange<T>> multirange, GaussDBRange<T> range)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(IsAdjacentTo)));

    #endregion IsAdjacentTo

    #region Union

    /// <summary>
    ///     Returns the set union, which means unique elements that appear in either of two multiranges.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>A multirange containing the unique elements that appear in either multirange.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Union{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static GaussDBRange<T>[] Union<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Union)));

    /// <summary>
    ///     Returns the set union, which means unique elements that appear in either of two multiranges.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>A multirange containing the unique elements that appear in either multirange.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Union{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static List<GaussDBRange<T>> Union<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Union)));

    #endregion Union

    #region Intersect

    /// <summary>
    ///     Returns the set intersection, which means elements that appear in each of two multiranges.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>A multirange containing the elements that appear in both ranges.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Intersect{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static GaussDBRange<T>[] Intersect<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Intersect)));

    /// <summary>
    ///     Returns the set intersection, which means elements that appear in each of two multiranges.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>A multirange containing the elements that appear in both ranges.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Intersect{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static List<GaussDBRange<T>> Intersect<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Intersect)));

    #endregion Intersect

    #region Except

    /// <summary>
    ///     Returns the set difference, which means the elements of one multirange that do not appear in a second multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>A multirange containing the elements that appear in the first range, but not the second range.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Except{T}(GaussDBRange{T}[], GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static GaussDBRange<T>[] Except<T>(this GaussDBRange<T>[] multirange1, GaussDBRange<T>[] multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Except)));

    /// <summary>
    ///     Returns the set difference, which means the elements of one multirange that do not appear in a second multirange.
    /// </summary>
    /// <param name="multirange1">The first multirange.</param>
    /// <param name="multirange2">The second multirange.</param>
    /// <returns>A multirange containing the elements that appear in the first range, but not the second range.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Except{T}(List{GaussDBRange{T}}, List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static List<GaussDBRange<T>> Except<T>(this List<GaussDBRange<T>> multirange1, List<GaussDBRange<T>> multirange2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Except)));

    #endregion Except

    #region Merge

    /// <summary>
    ///     Computes the smallest range that includes the entire multirange.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <returns>The smallest range that includes the entire multirange.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Merge{T}(GaussDBRange{T}[])" /> is only intended for use via SQL translation as part of an EF Core LINQ query.
    /// </exception>
    public static GaussDBRange<T> Merge<T>(this GaussDBRange<T>[] multirange)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Merge)));

    /// <summary>
    ///     Computes the smallest range that includes the entire multirange.
    /// </summary>
    /// <param name="multirange">The multirange.</param>
    /// <returns>The smallest range that includes the entire multirange.</returns>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Merge{T}(List{GaussDBRange{T}})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static GaussDBRange<T> Merge<T>(this List<GaussDBRange<T>> multirange)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Merge)));

    #endregion Merge
}
