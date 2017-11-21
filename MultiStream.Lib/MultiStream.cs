using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiStream.Lib
{
    public class MultiStream : Stream
    {
        private readonly Stream[] _streams;

        private int _index;
        private long _position;

        public MultiStream(IEnumerable<Stream> streams)
            : this(streams.ToArray())
        {
        }

        public MultiStream(params Stream[] streams)
        {
            _streams = streams;
            _index = _streams?.Length > 0 ? 0 : -1;
        }

        public override bool CanRead => _streams?.All(s => s.CanRead) ?? true;

        public override bool CanSeek => _streams?.All(s => s.CanSeek) ?? true;

        public override bool CanWrite => false;

        public override long Length => _streams?.Sum(s => s.Length) ?? 0;

        public override long Position
        {
            get => _position;

            set => Seek(value, SeekOrigin.Begin);
        }

        public override void Flush()
        {
            var current = 0;

            while(current <= _index)
            {
                _streams[current++].Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_index == -1)
            {
                return 0;
            }
            else
            {
                var totalBytesRead = 0;

                var bytesRead = _streams[_index].Read(buffer, offset, count);

                totalBytesRead += bytesRead;
                _position += bytesRead;

                while (bytesRead < count)
                {
                    if (_index == _streams.Length - 1)
                    {
                        break;
                    }

                    offset += bytesRead;
                    count -= bytesRead;
                    _index++;

                    bytesRead = _streams[_index].Read(buffer, offset, count);

                    totalBytesRead += bytesRead;
                    _position += bytesRead;
                }

                return totalBytesRead;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var length = Length;

            if (length == 0)
            {
                _position = 0;
                _index = -1;
            }
            else
            {
                var value = 0L;

                switch (origin)
                {
                    case SeekOrigin.Begin:
                        value = 0 + offset;
                        break;

                    case SeekOrigin.Current:
                        value = _position + offset;
                        break;

                    case SeekOrigin.End:
                        value = length + offset;
                        break;
                }

                if (value < 0)
                {
                    value = 0;
                }
                else if (value >= length)
                {
                    value = length - 1;
                }

                var accum = 0L;

                for (var i = 0; i < _streams.Length; i++)
                {
                    var current = _streams[i];

                    accum += current.Length;

                    if (accum > value)
                    {
                        _position = value;

                        _index = i;

                        _streams[_index].Position = _streams[_index].Length - (accum - value);

                        break;
                    }
                }
            }

            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
