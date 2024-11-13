using AutatoLib.Units;
using AutatoLib.Units.Composite;

namespace AutatoLib.Tests;

public class Tests {
    private Units.Conversions.TimeConverter timeConverter = new();
    private Time _second = new Time("s");
    private Time _minute = new Time("m");
    private Time _hour = new Time("h");
    private Time _day = new Time("d");
    private Time _week = new Time("w");
    private Time _millisecond = new Time("ms");
    private Time _year = new Time("y");

    [SetUp]
    public void Setup() {
        // month is a degenerate unit of time and shouldn't ever be used

        this.timeConverter.Add(this._millisecond);
        this.timeConverter.Add(this._second);
        this.timeConverter.Add(this._minute);
        this.timeConverter.Add(this._hour);
        this.timeConverter.Add(this._day);
        this.timeConverter.Add(this._week);
        this.timeConverter.Add(this._year);

        this.timeConverter.RegisterConverter(new TimePerTime(1000, this._millisecond, this._second));
        this.timeConverter.RegisterConverter(new TimePerTime(60, this._second, this._minute));
        this.timeConverter.RegisterConverter(new TimePerTime(60, this._minute, this._hour));
        this.timeConverter.RegisterConverter(new TimePerTime(24, this._hour, this._day));
        this.timeConverter.RegisterConverter(new TimePerTime(7, this._day, this._week));
        this.timeConverter.RegisterConverter(new TimePerTime(52, this._week, this._year));
    }

    [Test]
    public void MillisecondUnitConversions() {
        // The important thing here is transitivity.
        Assert.Multiple(() => {
            Assert.That(this.timeConverter.Convert(this._second, this._millisecond, 1), Is.EqualTo(1000d));
            Assert.That(this.timeConverter.Convert(this._minute, this._millisecond, 1), Is.EqualTo(1000d * 60));
            Assert.That(this.timeConverter.Convert(this._hour, this._millisecond, 1), Is.EqualTo(1000d * 60 * 60));
            Assert.That(this.timeConverter.Convert(this._day, this._millisecond, 1), Is.EqualTo(1000d * 60 * 60 * 24));
            Assert.That(this.timeConverter.Convert(this._week, this._millisecond, 1),
                Is.EqualTo(1000d * 60 * 60 * 24 * 7));
            Assert.That(this.timeConverter.Convert(this._year, this._millisecond, 1),
                Is.EqualTo(1000d * 60 * 60 * 24 * 7 * 52));
        });
    }

    [Test]
    public void MultipleMillisecondUnitConversions() {
        // And we can test quantities are being multiplied correctly.
        Assert.Multiple(() => {
            Assert.That(this.timeConverter.Convert(this._second, this._millisecond, 10), Is.EqualTo(1000d * 10));
            Assert.That(this.timeConverter.Convert(this._minute, this._millisecond, 10), Is.EqualTo(1000d * 60 * 10));
            Assert.That(this.timeConverter.Convert(this._hour, this._millisecond, 10),
                Is.EqualTo(1000d * 60 * 60 * 10));
            Assert.That(this.timeConverter.Convert(this._day, this._millisecond, 10),
                Is.EqualTo(1000d * 60 * 60 * 24 * 10));
            Assert.That(this.timeConverter.Convert(this._week, this._millisecond, 10),
                Is.EqualTo(1000d * 60 * 60 * 24 * 7 * 10));
            Assert.That(this.timeConverter.Convert(this._year, this._millisecond, 10),
                Is.EqualTo(1000d * 60 * 60 * 24 * 7 * 52 * 10));
        });
    }

    [Test]
    public void InverseUnitConversions() {
        // And inverse relations work too
        Assert.Multiple(() => {
            Assert.That(this.timeConverter.Convert(this._millisecond, this._second, 1), Is.EqualTo(1 / 1000d));
            Assert.That(this.timeConverter.Convert(this._second, this._minute, 1), Is.EqualTo(1 / 60d));
            Assert.That(this.timeConverter.Convert(this._minute, this._hour, 1), Is.EqualTo(1 / 60d));
            Assert.That(this.timeConverter.Convert(this._hour, this._day, 1), Is.EqualTo(1 / 24d));
            Assert.That(this.timeConverter.Convert(this._day, this._week, 1), Is.EqualTo(1 / 7d));
            Assert.That(this.timeConverter.Convert(this._week, this._year, 1), Is.EqualTo(1 / 52d));

            Assert.That(this.timeConverter.Convert(this._millisecond, this._year, 1), Is.EqualTo(1 / (1000d * 60 * 60 * 24 * 7 * 52)));
        });
    }
}