﻿using RetSim.Simulation.Events;

namespace RetSim.Simulation.EventQueues
{
    public class MinQueue : List<Event>, IEventQueue
    {
        private int minIndex = -1;
        public new void Add(Event e)
        {
            if (e != null)
            {
                base.Add(e);
                minIndex = -1;
            }
        }

        public void AddRange(List<Event> events)
        {
            foreach (Event e in events)
            {
                Add(e);
            }
        }

        public Event GetNext()
        {
            return this[minIndex];
        }

        public Event RemoveNext()
        {
            Event next = this[minIndex];
            RemoveAt(minIndex);
            minIndex = -1;
            return next;
        }

        public void EnsureSorting()
        {
            if (Count > 0 && minIndex == -1)
            {
                Event min = this[0];
                minIndex = 0;
                for (int i = 1; i < Count; i++)
                {
                    if (min.CompareTo(this[i]) >= 0)
                    {
                        min = this[i];
                        minIndex = i;
                    }
                }
            }
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void UpdateRemove(Event e)
        {
        }

        public void UpdateAdd(Event e)
        {
        }
    }
}