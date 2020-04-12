using System;

namespace C19QCalcLib
{
    public class Person
    {
        public string Name { get; private set; }
        public DateTime QuarantineStarted { get; private set; }
        public DateTime? FirstSymptoms { get; private set; }
        public double Temperature { get; private set; }

        public Person(string name, DateTime quarantineStarted, double temp, DateTime? firstSymptoms=null)
        {
            Name = name;
            QuarantineStarted = quarantineStarted;
            Temperature = temp;
            FirstSymptoms = firstSymptoms;
        }
    }
}
