using System.Collections.Generic;
using Sound;
using UnityEngine;

namespace Player
{
    public class SoundManager :  MonoBehaviour
    {
        private readonly List<AmbZone> _zones = new();

        public void AddZone(AmbZone zone)
        {
            if (!_zones.Contains(zone)) _zones.Add(zone);
            UpdateZones();
        }

        public void RemoveZone(AmbZone zone)
        {
            if (_zones.Remove(zone)) zone.FadeOut();
            UpdateZones();
        }
        
        private void UpdateZones()
        {
            if (_zones.Count == 0) return;
            
            AmbZone active = _zones[0];

            foreach (var zone in _zones)
            {
                if (zone.Priority > active.Priority) active = zone;
            }

            foreach (var zone in _zones)
            {
                if (zone == active) zone.FadeIn();
                else zone.FadeOut();
            }
        }
    }
}