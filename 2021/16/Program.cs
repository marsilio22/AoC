using System.Text;

var line = File.ReadAllLines("./input.txt")[0];

var binary = new StringBuilder();

foreach (var character in line)
{
    var numeric = Convert.ToInt32(character.ToString(), 16);

    string str = Convert.ToString(numeric, 2);

    binary.Append(str.PadLeft(4, '0'));
}

var fullString = binary.ToString();

int i = 0;
var packet = GetPacket(fullString, ref i);

Console.WriteLine(packet.SumVersions());
Console.WriteLine(packet.Value);

Packet GetPacket(string fullString, ref int i)
{
    var packet = new Packet();
    string version = string.Empty;
    string type = string.Empty;

    for (int j = 0; j < 3; j++)
    {
        version += fullString[i + j];
        type += fullString[i + j + 3];
    }

    i += 6;

    packet.Version = Convert.ToInt32(version, 2);
    packet.Type = Convert.ToInt32(type, 2);

    if (packet.Type == 4)
    {
        var valueString = string.Empty;
        while (fullString[i] == '1')
        {
            // add one to i
            i += 1;
            // add the next 4 bits to a binary string
            for (int j = 0; j < 4; j++)
            {
                valueString += fullString[i + j];
            }
            i += 4;
        }

        i += 1; // skip the 0
        // must be a 0, so read the last 4 bits of the number and parse
        for (int j = 0; j < 4; j++)
        {
            valueString += fullString[i + j];
        }
        i += 4;

        var value = Convert.ToInt64(valueString, 2);
        packet.Value = value;
    }
    else
    {
        var idType = fullString[i];
        i++;

        if (idType == '0')
        {
            // the next 15 bits are the total length in bits of the sub packets
            var value = string.Empty;
            for (int j = 0; j < 15; j++)
            {
                value += fullString[i + j];
            }
            i += 15;

            var subPacketLength = Convert.ToInt32(value, 2);

            var nextStop = i + subPacketLength; 

            while (i < nextStop)
            {
                packet.SubPackets.Add(GetPacket(fullString, ref i));
            }
        }
        else
        {
            // the next 11 bits are the number of sub packets in this packet
            var value = string.Empty;
            for (int j = 0; j < 11; j++)
            {
                value += fullString[i + j];
            }
            i += 11;

            var subPacketCount = Convert.ToInt32(value, 2);

            while (packet.SubPackets.Count() < subPacketCount)
            {
                packet.SubPackets.Add(GetPacket(fullString, ref i));
            }
        }
    }

    return packet;
}

public class Packet
{
    public int Version { get; set; }

    public int Type { get; set; }

    private long _value;

    public long Value
    {
        get
        {
            return this.GetValue();
        }
        set
        {
            this._value = value;
        }
    }

    public List<Packet> SubPackets { get; set; } = new List<Packet>();

    public long SumVersions()
    {
        return this.Version + this.SubPackets.Sum(s => s.SumVersions());
    }

    private long GetValue()
    {
        switch (this.Type)
        {
            case 0:
                return SubPackets.Sum(p => p.GetValue());
            case 1:
                return SubPackets.Select(v => v.GetValue()).Aggregate((p, q) => p * q);
            case 2:
                return SubPackets.Min(v => v.GetValue());
            case 3:
                return SubPackets.Max(v => v.GetValue());
            case 4:
                return this._value;
            case 5:
                return this.SubPackets[0].GetValue() > this.SubPackets[1].GetValue() ? 1 : 0;
            case 6:
                return this.SubPackets[0].GetValue() < this.SubPackets[1].GetValue() ? 1 : 0;
            case 7:
                return this.SubPackets[0].GetValue() == this.SubPackets[1].GetValue() ? 1 : 0;
            default:
                throw new Exception("oh noes");
        }
    }
}