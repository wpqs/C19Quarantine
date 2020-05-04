using System;

namespace C19QCalcLib
{
    public class Record
    {
        public string Name { get; private set; }
        public DateTime QuarantineStartedUtc { get; private set; }
        public DateTime? FirstSymptomsUtc { get; private set; }
        public double Temperature { get; private set; }

        public void SetFirstSymptoms(DateTime firstSymptomsUtc) { FirstSymptomsUtc = firstSymptomsUtc; }
        public Record(string name, DateTime quarantineStartedUtc, double temp, DateTime? firstSymptomsUtc=null)
        {
            Name = name;
            QuarantineStartedUtc = quarantineStartedUtc;
            Temperature = temp;
            FirstSymptomsUtc = firstSymptomsUtc;
        }
    }
}
