using System.Collections.Generic;
using UnityEngine;

namespace Elias.Scripts.Helper
{
    public static class InputHelpers
    {
        private static readonly KeyCode[] Keys = { KeyCode.R, KeyCode.G, KeyCode.B };
        public static readonly List<KeyCode> InputList = new(Keys);
    }
}