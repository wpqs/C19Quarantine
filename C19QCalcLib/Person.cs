using System;

namespace C19QCalcLib
{
    public class Person
    {
        public string Name { get; private set; }
        public DateTime QuarantineStartedUtc { get; private set; }
        public DateTime? FirstSymptomsUtc { get; private set; }
        public double Temperature { get; private set; }

        public Person(string name, DateTime quarantineStartedUtc, double temp, DateTime? firstSymptomsUtc=null)
        {
            Name = name;
            QuarantineStartedUtc = quarantineStartedUtc;
            Temperature = temp;
            FirstSymptomsUtc = firstSymptomsUtc;
        }
    }
}
