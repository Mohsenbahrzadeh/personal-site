namespace PersonalSite.Data;

/// <summary>
/// Reads the duration of an MP4/MOV file by walking the ISO-BMFF box tree
/// (moov -&gt; mvhd) directly, without depending on ffmpeg or any external tool.
/// </summary>
public static class Mp4DurationReader
{
    public static TimeSpan? TryReadDuration(Stream stream)
    {
        try
        {
            return FindMvhdDuration(stream, stream.Length);
        }
        catch
        {
            return null;
        }
    }

    private static TimeSpan? FindMvhdDuration(Stream stream, long endPosition)
    {
        while (stream.Position + 8 <= endPosition)
        {
            long boxStart = stream.Position;
            uint size32 = ReadUInt32BE(stream);
            string type = ReadFourCC(stream);

            long headerSize = 8;
            long boxSize = size32;

            if (size32 == 1)
            {
                boxSize = (long)ReadUInt64BE(stream);
                headerSize = 16;
            }
            else if (size32 == 0)
            {
                boxSize = endPosition - boxStart;
            }

            long boxEnd = boxStart + boxSize;
            if (boxSize < headerSize || boxEnd > endPosition)
            {
                break;
            }

            if (type == "moov")
            {
                var result = FindMvhdDuration(stream, boxEnd);
                if (result is not null)
                {
                    return result;
                }
            }
            else if (type == "mvhd")
            {
                return ReadMvhdPayload(stream);
            }

            stream.Position = boxEnd;
        }

        return null;
    }

    private static TimeSpan? ReadMvhdPayload(Stream stream)
    {
        int version = stream.ReadByte();
        stream.Seek(3, SeekOrigin.Current); // flags

        long timescale;
        long duration;

        if (version == 1)
        {
            stream.Seek(8 + 8, SeekOrigin.Current); // creation + modification time (64-bit each)
            timescale = ReadUInt32BE(stream);
            duration = (long)ReadUInt64BE(stream);
        }
        else
        {
            stream.Seek(4 + 4, SeekOrigin.Current); // creation + modification time (32-bit each)
            timescale = ReadUInt32BE(stream);
            duration = ReadUInt32BE(stream);
        }

        if (timescale <= 0)
        {
            return null;
        }

        return TimeSpan.FromSeconds((double)duration / timescale);
    }

    private static uint ReadUInt32BE(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
    }

    private static ulong ReadUInt64BE(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        ulong value = 0;
        foreach (byte b in buffer)
        {
            value = (value << 8) | b;
        }
        return value;
    }

    private static string ReadFourCC(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return System.Text.Encoding.ASCII.GetString(buffer);
    }
}
