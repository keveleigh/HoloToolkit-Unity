// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class FocusProviderExtensions
    {
        //public static void GetPointers<T>(this IMixedRealityFocusProvider focusProvider, List<T> pointersList) where T : class, IMixedRealityPointer
        //{
        //    pointersList.Clear();
        //    pointersList.AddRange(focusProvider.GetPointers<T>());
        //}

        //public static T GetFirstPointerWhere<T>(this IMixedRealityFocusProvider focusProvider, Predicate<T> predicate) where T : class, IMixedRealityPointer
        //{
        //    IEnumerable<T> pointers = focusProvider.GetPointers<T>();
        //    foreach (T pointer in pointers)
        //    {
        //        if (predicate(pointer))
        //        {
        //            return pointer;
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Gets the first registered pointer that matches a given handedness.
        ///// </summary>
        ///// <typeparam name="T">The type of pointers to request. Use IMixedRealityPointer to access all pointers.</typeparam>
        ///// <param name="handedness">The handedness being requested.</param>
        ///// <returns>The first pointer in the collection that matches the specified handedness.</returns>
        //public static T GetFirstPointerWithHandedness<T>(this IMixedRealityFocusProvider focusProvider, Handedness handedness) where T : class, IMixedRealityPointer
        //{
        //    IEnumerable<T> pointers = focusProvider.GetPointers<T>();
        //    foreach (T pointer in pointers)
        //    {
        //        if (pointer.Controller?.ControllerHandedness.IsMatch(handedness) == true)
        //        {
        //            return pointer;
        //        }
        //    }
        //    return null;
        //}
    }
}
