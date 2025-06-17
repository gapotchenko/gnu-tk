// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#pragma warning disable IDE0032 // Use auto property

namespace Gapotchenko.GnuTK.IO;

/// <summary>
/// Represents a reader that can track the current position in a sequential series of characters.
/// </summary>
sealed class PositionTrackingTextReader : TextReader
{
    public PositionTrackingTextReader(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        m_BaseReader = reader;
    }

    public override int Read()
    {
        int result = m_BaseReader.Read();
        if (result != -1)
            ++m_Position;
        return result;
    }

    public override int Read(char[] buffer, int index, int count)
    {
        int charsRead = m_BaseReader.Read(buffer, index, count);
        if (charsRead > 0)
            m_Position += charsRead;
        return charsRead;
    }

    public long Position => m_Position;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    long m_Position;

    public override void Close() => m_BaseReader.Close();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            m_BaseReader.Dispose();
        base.Dispose(disposing);
    }

    public TextReader BaseReader => m_BaseReader;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly TextReader m_BaseReader;
}
