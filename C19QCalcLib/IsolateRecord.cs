using NodaTime;

namespace C19QCalcLib
{
    public class IsolateRecord
    {
        public string Name { get; private set; }
        public Instant QuarantineStarted { get; private set; }
        public Instant? FirstSymptoms { get; private set; }
        public bool HasSymptoms { get; private set; }

        public void SetFirstSymptoms(Instant firstSymptomsUtc) { FirstSymptoms = firstSymptomsUtc; }
        public IsolateRecord(string name, Instant quarantineStarted, bool hasSymptoms, Instant? firstSymptoms=null)
        {
            Name = name;
            QuarantineStarted = quarantineStarted;
            HasSymptoms = hasSymptoms;
            FirstSymptoms = firstSymptoms;
        }
    }
}
