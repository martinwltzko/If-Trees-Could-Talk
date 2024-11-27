using Code.Scripts.UI;
using UnityEngine;

namespace UnityUtils.Aiming
{
    public interface IAimingTarget
    {
        //public OptionProvider OptionProvider { get; }
        public Transform Transform { get; }
        public void OnAimingStart(object sender);
        public void OnAimingEnd(object sender);
    }
}