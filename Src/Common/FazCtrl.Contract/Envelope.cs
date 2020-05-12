using System;

namespace FazCtrl.Contract
{
    public abstract class Envelope
    {
        public static Envelope<TEvent> Create<TEvent>(TEvent body)
        {
            return new Envelope<TEvent>(body);
        }
    }

    public class Envelope<TEvent> : Envelope
    {
        public Envelope(TEvent body)
        {
            Body = body;
        }

        public TEvent Body { get; private set; }
        public TimeSpan Delay { get; set; }
        public TimeSpan TimeToLive { get; set; }
        public string CorrelationId { get; set; }
        public string MessageId { get; set; }
        public string Slip { get; set; }
        public string Tenant { get; set; }
        public Guid Property { get; set; }

        public static implicit operator Envelope<TEvent>(TEvent body)
        {
            return Create(body);
        }
    }
}
