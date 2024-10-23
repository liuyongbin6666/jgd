using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components
{
    public class VisionEmitComponent : TrackingComponentBase 
    {
        List<VisionActiveComponent> components = new();
        protected override void OnTrackingEnter(GameObject go)
        {
            components = components.Where(c => c).ToList();
            var vision = go.GetComponent<VisionActiveComponent>();
            if (vision == null) return;
            if(components.Contains(vision))return;
            vision.SetActive(true);
            components.Add(vision);
        }

        protected override void OnTrackingExit(GameObject go)
        {
            var vision = go.GetComponent<VisionActiveComponent>();
            if (vision == null) return;
            vision.SetActive(false);
            components.Remove(vision);
        }
    }
}