/*
 * Copyright (c) 2024 JuicyGrape Apps.
 *
 * Licensed under the MIT License, (the "License");
 * you may not use any file by JuicyGrape Apps except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.juicygrapeapps.com/terms
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JuicyGrapeApps.Core
{
    public static class Scroll
    {
        private static Dictionary<int, int> arrayIndex = new Dictionary<int, int>();
        private static Action<int, int> rescan;
        public static int lastIndex = 0;

        // Scrolling action to perform.
        public enum ScrollingAction
        {
            Continuous,
            Directional,
            Targeted,
        }

        /// <summary>
        /// A singlare function to scan any collection types like arrays and lists in either direction. 
        /// this function remembers the index of each collection scanned by storing it in a dictionary above.
        /// 
        /// Overloading methods <see cref="Collection(object, bool, ScrollingAction)"/>
        ///                     <see cref="Collection(Transform, int, ScrollingAction)"/>
        /// </summary>
        /// 
        /// <param name="collection"></param> -  The collection being scanned. 
        /// <param name="step"></param> - Scan through collections indicies at this rate, negative values scan backwards and a zero value will return current index. 
        /// <param name="action"></param> - Scrolling action to perform, Continuous - Scrolls continuosly looping from start to end or vica-versa, Directional stops when beginning or end reached.
        ///                                 and Targeted targets a specific index specified by step value.  <see cref="SetIndex(object, int)"/>
        ///
        /// Useage: Use this function instead of specifiying the index of a collection. 
        /// 
        /// <returns>Intager value of a collections index</returns>
        public static int Collection(object collection, int step = 1, ScrollingAction action = ScrollingAction.Continuous)
        {
            try
            {
                Type type = collection.GetType();
                int length = ((int)(type.GetProperty("Length") ?? type.GetProperty("Count"))?.GetValue(collection)) - 1;
                if (length < 0) return 0;
                // Retrive collection's index from 'arrayIndex' dictionary or set index to zero
                int store = retriveIndex(collection, action);
                int index = store + step;
                if (step == 0 || (action != ScrollingAction.Continuous && (index < 0 || index > length))) return store;
                index = (index < 0) ? length : (index > length) ? 0 : index;
                // Save current collection's index in 'arrayIndex' dictionary
                storeIndex(collection, index);
                lastIndex = index;

                if (rescan != null)
                    for (int idx = 0; idx < length; idx++) rescan.Invoke(index, idx);

                return index;
            }
            catch { return 0; }
        }

        /// <summary>
        /// Call back method for the rescan of collection, no recan will be preformed if
        /// this listener is not set.
        /// </summary>
        /// <param name="value"></param>
        internal static void OnRescanListener(Action<int, int> value) => rescan = value;

        /// Usage: Use this overload to scan the children of GameObject;
        /// 
        /// <returns>Transform of child object at index or null if no children exist</returns>
        public static Transform Collection(Transform collection, int step = 1, ScrollingAction action = ScrollingAction.Continuous)
        {
            try
            {
                int length = collection.childCount - 1;
                if (length < 0) return null;
                // Retrive child object's index from 'arrayIndex' dictionary or set index to zero
                int store = retriveIndex(collection, action);
                int index = store + step;
                if (step == 0 || (action != ScrollingAction.Continuous && (index < 0 || index > length))) return collection.GetChild(store);
                index = (index < 0) ? length : (index > length) ? 0 : index;
                // Save current child object's index to 'arrayIndex' dictionary 
                storeIndex(collection, index);
                lastIndex = index;
                return collection.GetChild(index);
            }
            catch { return null; }
        }

        private static void storeIndex(object collection, int index)
        {
            // Store the objects hashcode only to prevent memory leaks
            int Id = collection.GetHashCode();
            arrayIndex[Id] = index;
        }

        private static int retriveIndex(object collection, ScrollingAction action)
        {
            if (action == ScrollingAction.Targeted) return 0;
            int Id = collection.GetHashCode();
            arrayIndex.TryGetValue(Id, out int index);
            return index;
        }


#region - Simplified overload 
        /// Usage: Use this overload to simply scan upwards/backward though indexes.
        public static int Up(object collection) => Collection(collection, -1, ScrollingAction.Continuous);
        /// Usage: Use this overload to simply scan upwards/backward though the children of a GameObject.
        public static Transform Up(Transform collection) => Collection(collection, -1, ScrollingAction.Continuous);
        /// Usage: Use this overload to simply scan down/forward though indexes.
        public static int Down(object collection) => Collection(collection, 1, ScrollingAction.Continuous);
        /// Usage: Use this overload to simply scan downwards/forward though the children of a GameObject.
        public static Transform Down(Transform collection) => Collection(collection, 1, ScrollingAction.Continuous);
        /// Usage: Use this overload to use a boolean value to scan one step forward (true) or backward (false).
        public static int Collection(object collection, bool forward, ScrollingAction action = ScrollingAction.Continuous) => Collection(collection, forward ? 1 : -1, action);
        /// Usage: Use this overload to use a boolean value to scan the children of a GameObject, forward (true) or backward (false).
        public static Transform Collection(Transform collection, bool forward, ScrollingAction action = ScrollingAction.Continuous) => Collection(collection, forward ? 1 : -1, action);
        #endregion

#region - Get/set the indexes of a collection
        /// Usage: Use this function to target an index in a collection.
        public static int GetIndex(object collection) => Collection(collection, 0);

        /// Usage: Use this function to get the index a GameObject's child.
        public static int GetIndex(Transform collection, Transform transform)
        {
            SetIndex(collection, transform);
            return lastIndex;
        }

        /// Usage: Use this function to get the child's transform at a specified index within a GameObject.
        public static Transform GetIndex(Transform collection, int index) => Collection(collection, index, ScrollingAction.Targeted);

        /// Usage: Use this function to target an index in a collection, use with lastIndex to sync different arrays with same lengths.
        public static void SetIndex(object collection, int index) => Collection(collection, index, ScrollingAction.Targeted);
        

        /// Usage: Use this function to target a specific child of a GameObject.
        public static void SetIndex(Transform collection, Transform transform)
        {
            int index = 0;
            foreach (Transform t in collection) { if (t == transform) break; index++; }
            Collection(collection, index, ScrollingAction.Targeted);
        }
#endregion

    }
}
